using System.Reflection;
using GHLearning.EasyQuartzDashboard.Application.Abstractions.Behaviors;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
		=> services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

			config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
			config.AddOpenBehavior(typeof(HandleTracingPipelineBehavior<,>));
		});
}