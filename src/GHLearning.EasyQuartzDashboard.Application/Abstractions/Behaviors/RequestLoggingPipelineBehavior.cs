using MediatR;
using Microsoft.Extensions.Logging;

namespace GHLearning.EasyQuartzDashboard.Application.Abstractions.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
	ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;

		logger.LogInformation("Processing request {RequestName}", requestName);

		try
		{
			var result = await next(cancellationToken).ConfigureAwait(false);

			logger.LogInformation("Completed request {RequestName}", requestName);

			return result;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error processing request {RequestName}", requestName);
			throw;
		}
	}
}