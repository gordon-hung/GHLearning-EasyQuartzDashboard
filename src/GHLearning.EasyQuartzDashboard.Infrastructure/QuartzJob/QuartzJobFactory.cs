using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob;

public class QuartzJobFactory(IServiceProvider serviceProvider) : IJobFactory
{
	public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
	{
		var jobType = bundle.JobDetail.JobType;

		// 從 DI 容器取出指定 Job Type 實體，並檢查是否為 null
		return serviceProvider.GetRequiredService(jobType) is not IJob jobInstance
			? throw new InvalidOperationException($"The job instance of type {jobType.FullName} could not be resolved.")
			: jobInstance;
	}

	public void ReturnJob(IJob job)
	{
		var disposable = job as IDisposable;
		disposable?.Dispose();
	}
}