using GHLearning.EasyQuartzDashboard.Web.Hubs;
using Quartz;

namespace GHLearning.EasyQuartzDashboard.Web.Listeners;

public class QuartzJobListener(
	ILogger<QuartzJobListener> logger,
	IServiceProvider serviceProvider,
	TimeProvider timeProvider) : IJobListener
{
	string IJobListener.Name => "Jobs Listener";

	public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
	{
		// 工作將被執行 (目前Job狀態尚未進入Executing清單)

		var jobName = context.JobDetail.Key.Name;
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Activity:{activity}", timeProvider.GetUtcNow(), jobName, nameof(JobToBeExecuted));

		var schedulerHub = serviceProvider.GetRequiredService<SchedulerHub>();
		await schedulerHub.NotifyJobStatusChange().ConfigureAwait(false);
	}

	public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
	{
		// 工作執行完畢 (目前Job狀態尚未移出Executing清單)

		var jobName = context.JobDetail.Key.Name;
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Activity:{activity}", timeProvider.GetUtcNow(), jobName, nameof(JobWasExecuted));

		var schedulerHub = serviceProvider.GetRequiredService<SchedulerHub>();
		await schedulerHub.NotifyJobStatusChange().ConfigureAwait(false);
	}

	public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
	{
		var jobName = context.JobDetail.Key.Name;
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Activity:{activity}", timeProvider.GetUtcNow(), jobName, nameof(JobExecutionVetoed));

		return Task.CompletedTask;
	}
}