using GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob;
using GHLearning.EasyQuartzDashboard.Web.Extension;
using GHLearning.EasyQuartzDashboard.Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace GHLearning.EasyQuartzDashboard.Web.Hubs;

/// <summary>
/// 建構子
/// </summary>
/// <param name="quartzHostedService">Quartz排程服務</param>
public class SchedulerHub(QuartzHostedService quartzHostedService) : Hub
{
	/// <summary>
	/// Requests the job status.
	/// </summary>
	public async Task RequestJobStatus()
	{
		if (Clients != null)
		{
			var dtos = await quartzHostedService.GetJobSchedules().ConfigureAwait(false);
			var viewModels = dtos.Select(job => new JobInfoViewModel(
				JobName: job.JobName,
				JobDesc: job.JobDesc,
				JobType: job.JobType.FullName ?? string.Empty,
				JobCronExpression: job.JobCronExpression,
				JobCronExpressionDes: job.JobCronExpressionDes,
				JobStatus: job.JobStatus,
				JobStatusName: job.JobStatus.GetDescription(),
				StartTime: job.StartTime,
				EndTime: job.EndTime,
				PreviousFireTime: job.PreviousFireTime,
				NextFireTime: job.NextFireTime)).ToList();
			await Clients.Caller.SendAsync("ReceiveJobStatus", viewModels).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Notifies the job status change.
	/// </summary>
	public async Task NotifyJobStatusChange()
	{
		if (Clients != null)
		{
			await Clients.All.SendAsync("JobStatusChange").ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Triggers the job.
	/// </summary>
	/// <param name="jobName">Name of the job.</param>
	public async Task TriggerJob(string jobName)
	{
		await quartzHostedService.TriggerJobAsync(jobName).ConfigureAwait(false);
	}

	/// <summary>
	/// Interrupts the job.
	/// </summary>
	/// <param name="jobName">Name of the job.</param>
	public async Task InterruptJob(string jobName)
	{
		await quartzHostedService.InterruptJobAsync(jobName).ConfigureAwait(false);
	}

	/// <summary>
	/// Starts the scheduler.
	/// </summary>
	public async Task StartScheduler()
	{
		await quartzHostedService.StartAsync(quartzHostedService.CancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Stops the scheduler.
	/// </summary>
	public async Task StopScheduler()
	{
		await quartzHostedService.StopAsync(quartzHostedService.CancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Called when a new connection is established with the hub.
	/// </summary>
	public override async Task OnConnectedAsync()
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users").ConfigureAwait(false);
		await NotifyJobStatusChange().ConfigureAwait(false);
		await base.OnConnectedAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Called when a connection with the hub is terminated.
	/// </summary>
	/// <param name="exception"></param>
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users").ConfigureAwait(false);
		await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
	}
}