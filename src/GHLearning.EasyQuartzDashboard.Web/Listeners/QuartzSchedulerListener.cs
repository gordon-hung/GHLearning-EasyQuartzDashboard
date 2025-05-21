using GHLearning.EasyQuartzDashboard.Web.Hubs;
using Quartz;

namespace GHLearning.EasyQuartzDashboard.Web.Listeners;

public class QuartzSchedulerListener(
	ILogger<QuartzSchedulerListener> logger,
	IServiceProvider serviceProvider,
	TimeProvider timeProvider) : ISchedulerListener
{
	public async Task SchedulerShutdown(CancellationToken cancellationToken = default)
	{
		logger.LogInformation("Time:{timeAt} - Activity:{activity}", timeProvider.GetUtcNow(), nameof(SchedulerShutdown));
		var schedulerHub = serviceProvider.GetRequiredService<SchedulerHub>();
		await schedulerHub.NotifyJobStatusChange().ConfigureAwait(false);
	}

	public async Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
	{
		logger.LogInformation("Time:{timeAt} - Activity:{activity}", timeProvider.GetUtcNow(), nameof(SchedulerShuttingdown));
		var schedulerHub = serviceProvider.GetRequiredService<SchedulerHub>();
		await schedulerHub.NotifyJobStatusChange().ConfigureAwait(false);
	}

	public async Task SchedulerStarted(CancellationToken cancellationToken = default)
	{
		logger.LogInformation("Time:{timeAt} - Activity:{activity}", timeProvider.GetUtcNow(), nameof(SchedulerStarted));
		var schedulerHub = serviceProvider.GetRequiredService<SchedulerHub>();
		await schedulerHub.NotifyJobStatusChange().ConfigureAwait(false);
	}

	public async Task SchedulerStarting(CancellationToken cancellationToken = default)
	{
		logger.LogInformation("Time:{timeAt} - Activity:{activity}", timeProvider.GetUtcNow(), nameof(SchedulerStarting));
		var schedulerHub = serviceProvider.GetRequiredService<SchedulerHub>();
		await schedulerHub.NotifyJobStatusChange().ConfigureAwait(false);
	}

	public Task SchedulingDataCleared(CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task TriggersPaused(string? triggerGroup, CancellationToken cancellationToken = default) => Task.CompletedTask;

	public Task TriggersResumed(string? triggerGroup, CancellationToken cancellationToken = default) => Task.CompletedTask;

	Task ISchedulerListener.JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobDeleted(JobKey jobKey, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobInterrupted(JobKey jobKey, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobPaused(JobKey jobKey, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobResumed(JobKey jobKey, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobScheduled(ITrigger trigger, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobsPaused(string jobGroup, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobsResumed(string jobGroup, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken) => Task.CompletedTask;

	Task ISchedulerListener.SchedulerInStandbyMode(CancellationToken cancellationToken) => Task.CompletedTask;
}