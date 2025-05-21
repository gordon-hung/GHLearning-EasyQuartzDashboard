using System.Diagnostics;
using MediatR;

namespace GHLearning.EasyQuartzDashboard.Application.Abstractions.Behaviors;

public class HandleTracingPipelineBehavior<TRequest, TResponse>(
	ActivitySource activitySource) : IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		using var activity = activitySource.StartActivity($"MediatR Handle");

		activity?.SetTag("TRequest", typeof(TRequest).Name);
		var response = await next(cancellationToken).ConfigureAwait(false);
		return response;
	}
}