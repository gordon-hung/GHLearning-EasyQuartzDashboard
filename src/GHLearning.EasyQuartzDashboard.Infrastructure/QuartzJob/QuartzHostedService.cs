using GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.Models;
using GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.Models.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob;

public class QuartzHostedService(
	ILogger<QuartzHostedService> logger,
	ISchedulerFactory schedulerFactory,
	IJobFactory jobFactory,
	IEnumerable<JobCreation> jobCreations,
	IJobListener jobListener,
	ISchedulerListener schedulerListener,
	TimeProvider timeProvider) : IHostedService
{
	private List<JobEntity>? _jobs;

	public IScheduler? Scheduler { get; set; }

	public CancellationToken CancellationToken { get; private set; }

	/// <summary>
	/// Triggered when the application host is ready to start the service.
	/// </summary>
	/// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		if (Scheduler == null || Scheduler.IsShutdown)
		{
			// 存下 cancellation token
			CancellationToken = cancellationToken;

			// 先加入在 startup 註冊注入的 Job 工作
			_jobs = [.. jobCreations.Select(x => new JobEntity
			(
				JobName: x.JobName,
				JobType: x.JobType,
				JobDesc: x.JobDesc,
				JobCronExpression: x.JobCronExpression,
				JobCronExpressionDes: x.JobCronExpressionDes
			))];

			// 初始排程器 Scheduler
			Scheduler = await schedulerFactory.GetScheduler(cancellationToken).ConfigureAwait(false);
			Scheduler.JobFactory = jobFactory;
			Scheduler.ListenerManager.AddJobListener(jobListener);
			Scheduler.ListenerManager.AddSchedulerListener(schedulerListener);

			// 逐一將工作項目加入排程器中
			foreach (var job in _jobs)
			{
				var jobDetail = CreateJobDetail(job);
				var trigger = CreateTrigger(job);
				await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken).ConfigureAwait(false);
			}

			// 啟動排程
			await Scheduler.Start(cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Triggered when the application host is performing a graceful shutdown.
	/// </summary>
	/// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
	public async Task StopAsync(CancellationToken cancellationToken)
	{
		if (Scheduler != null && !Scheduler.IsShutdown)
		{
			logger.LogInformation("Time:{timeAt} - Activity:{activity}", timeProvider.GetUtcNow(), nameof(StopAsync));
			await Scheduler.Shutdown(cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Gets the job schedules.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Scheduler is not initialized.</exception>
	public async Task<IEnumerable<JobInfo>> GetJobSchedules()
	{
		if (_jobs == null)
			return [];

		if (Scheduler == null)
			return [];

		var jobInfos = new List<JobInfo>();
		if (Scheduler.IsShutdown)
		{
			jobInfos.AddRange(_jobs
				.OrderBy(job => job.JobName)
				.Select(job => new JobInfo(
					JobName: job.JobName,
					JobDesc: job.JobDesc,
					JobType: job.JobType,
					JobCronExpression: job.JobCronExpression,
					JobCronExpressionDes: job.JobCronExpressionDes,
					JobStatus: JobStatus.Stopped,
					StartTime: null,
					EndTime: null,
					PreviousFireTime: null,
					NextFireTime: null)));
		}
		else
		{
			var jobKeys = new List<JobKey>();        //以 JobKey 存放所有的 Job, 每一個 Job 都有一個雖一的 JobKey
			var groupNames = await Scheduler.GetJobGroupNames().ConfigureAwait(false); //取得排程器中所有已知的 IJobDetail group

			// 以 groupname 取得 JobKey放入 jobKeyList 中
			foreach (var groupName in groupNames.OrderBy(groupName => groupName))
			{
				jobKeys.AddRange(await Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)).ConfigureAwait(false));
			}

			foreach (var jobKey in jobKeys.OrderBy(t => t.Name))
			{
				var jobDetail = await Scheduler.GetJobDetail(jobKey).ConfigureAwait(false);  //以JobKey 取得 JobDetail
				if (jobDetail == null)
					continue;

				var triggersList = await Scheduler.GetTriggersOfJob(jobKey).ConfigureAwait(false);  //以JobKey取得所有 Trigger(一個 Job 可以有多個 Trigger)
				var triggers = triggersList.AsEnumerable().FirstOrDefault();  //從 Trigger 清單中取得第一個 Trigger
				if (triggers == null)
					continue;

				// 檢查 trigger 的類型, 根據不同的類型給予 interval 不同的值
				var interval = triggers is SimpleTriggerImpl
					? ((triggers as SimpleTriggerImpl)?.RepeatInterval.ToString())
					: ((triggers as CronTriggerImpl)?.CronExpressionString);

				var jobExecution = await GetExecutingJob(jobKey.Name!).ConfigureAwait(false);

				// 將每一筆 Job 的明細資料放入 jobInfoLst 中
				jobInfos.Add(new JobInfo(
					JobName: jobKey.Name ?? string.Empty,
					JobType: jobDetail.JobType ?? typeof(IJob),
					JobCronExpression: interval ?? string.Empty,
					JobCronExpressionDes: triggers?.Description ?? interval ?? string.Empty,
					JobDesc: jobDetail.Description ?? string.Empty,
					JobStatus: jobExecution is not null ? JobStatus.Running : JobStatus.Scheduled,
					StartTime: triggers?.StartTimeUtc.LocalDateTime,
					EndTime: triggers?.EndTimeUtc?.LocalDateTime,
					PreviousFireTime: triggers?.GetPreviousFireTimeUtc()?.LocalDateTime,
					NextFireTime: triggers?.GetNextFireTimeUtc()?.LocalDateTime));
			}
		}

		return jobInfos;
	}

	/// <summary>
	/// Triggers the job asynchronous.
	/// </summary>
	/// <param name="jobName">Name of the job.</param>
	public async Task TriggerJobAsync(string jobName)
	{
		if (Scheduler != null && !Scheduler.IsShutdown)
		{
			logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Activity:{activity}", timeProvider.GetUtcNow(), jobName, nameof(TriggerJobAsync));
			await Scheduler.TriggerJob(new JobKey(jobName), CancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Interrupts the job asynchronous.
	/// </summary>
	/// <param name="jobName">Name of the job.</param>
	public async Task InterruptJobAsync(string jobName)
	{
		if (Scheduler != null && !Scheduler.IsShutdown)
		{
			var targetExecutingJob = await GetExecutingJob(jobName).ConfigureAwait(false);
			if (targetExecutingJob != null)
			{
				logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Activity:{activity}", timeProvider.GetUtcNow(), jobName, nameof(InterruptJobAsync));
				await Scheduler.Interrupt(new JobKey(jobName)).ConfigureAwait(false);
			}
		}
	}

	/// <summary>
	/// Gets the executing job.
	/// </summary>
	/// <param name="jobName">Name of the job.</param>
	/// <returns></returns>
	private async Task<IJobExecutionContext?> GetExecutingJob(string jobName)
	{
		if (Scheduler != null)
		{
			var executingJobs = await Scheduler.GetCurrentlyExecutingJobs().ConfigureAwait(false);
			return executingJobs.FirstOrDefault(j => j.JobDetail.Key.Name == jobName);
		}

		return null;
	}

	/// <summary>
	/// Creates the job detail.
	/// </summary>
	/// <param name="jobInfoEntity">The job information entity.</param>
	/// <returns></returns>
	private static IJobDetail CreateJobDetail(JobEntity jobInfoEntity)
	{
		var jobType = jobInfoEntity.JobType;
		var jobDetail = JobBuilder
			.Create(jobType)
			.WithIdentity(jobInfoEntity.JobName)
			.WithDescription(jobType.Name)
			.Build();

		jobDetail.JobDataMap.Put("Entity", jobInfoEntity);

		return jobDetail;
	}

	/// <summary>
	/// Creates the trigger.
	/// </summary>
	/// <param name="schedule">The schedule.</param>
	/// <returns></returns>
	private static ITrigger CreateTrigger(JobEntity schedule) => TriggerBuilder
		.Create()
		.WithIdentity($"{schedule.JobName}.trigger")
		.WithCronSchedule(schedule.JobCronExpression)
		.WithDescription(schedule.JobCronExpressionDes)
		.Build();
}