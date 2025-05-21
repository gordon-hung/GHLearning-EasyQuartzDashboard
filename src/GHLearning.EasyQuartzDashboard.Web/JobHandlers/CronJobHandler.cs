using GHLearning.EasyQuartzDashboard.Application.Customers.Add;
using MediatR;
using Quartz;

namespace GHLearning.EasyQuartzDashboard.Web.JobHandlers;

[DisallowConcurrentExecution]
public class CronJobHandler(
	ILogger<CronJobHandler> logger,
	IServiceProvider serviceProvider,
	TimeProvider timeProvider) : IJob
{
	private int _count = 0;

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Start", timeProvider.GetUtcNow(), context.JobDetail.Key.Name);
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Working", timeProvider.GetUtcNow(), context.JobDetail.Key.Name);
		_count++;
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Count:{count}", timeProvider.GetUtcNow(), context.JobDetail.Key.Name, _count);
		using (var sp = serviceProvider.CreateScope())
		{
			var mediator = sp.ServiceProvider.GetRequiredService<IMediator>();
			await mediator.Send(new CustomerAddRequest(context.JobDetail.Key.Name), context.CancellationToken).ConfigureAwait(false);
		}

		await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken).ConfigureAwait(false);
		logger.LogInformation("Time:{timeAt} - JobName:{jobName} - Done", timeProvider.GetUtcNow(), context.JobDetail.Key.Name);
	}
}