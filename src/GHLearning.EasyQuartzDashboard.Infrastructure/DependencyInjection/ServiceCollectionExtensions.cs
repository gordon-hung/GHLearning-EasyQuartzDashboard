using GHLearning.EasyQuartzDashboard.Core.Customers;
using GHLearning.EasyQuartzDashboard.Infrastructure;
using GHLearning.EasyQuartzDashboard.Infrastructure.Customers;
using GHLearning.EasyQuartzDashboard.Infrastructure.Entities;
using GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob;
using GHLearning.EasyQuartzDashboard.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> DbContextOptions)
		=> services
		.AddDbContext<EasyDbContext>(DbContextOptions)
		.AddSingleton<ISequentialGuidGenerator, SequentialGuidGenerator>()
		.AddTransient<ICustomerRepository, CustomerRepository>()
		.AddQuartzInfrastructure();

	private static IServiceCollection AddQuartzInfrastructure(
		this IServiceCollection services)
		=> services.AddSingleton<IJobFactory, QuartzJobFactory>()
		.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
}