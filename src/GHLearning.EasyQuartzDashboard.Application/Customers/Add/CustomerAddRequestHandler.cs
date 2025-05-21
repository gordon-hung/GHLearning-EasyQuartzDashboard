using GHLearning.EasyQuartzDashboard.Core.Customers;
using GHLearning.EasyQuartzDashboard.Core.Customers.Models;
using GHLearning.EasyQuartzDashboard.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GHLearning.EasyQuartzDashboard.Application.Customers.Add;

internal class CustomerAddRequestHandler(
	ILogger<CustomerAddRequestHandler> logger,
	TimeProvider timeProvider,
	ISequentialGuidGenerator sequentialGuidGenerator,
	ICustomerRepository customerRepository) : IRequestHandler<CustomerAddRequest>
{
	public Task Handle(CustomerAddRequest request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Time:{timeAt} - Activity:{activity}", timeProvider.GetUtcNow(), nameof(Handle));

		return customerRepository.AddAsync(
			parameter: new CustomerAddParameter(
				Name: request.Name,
				Order: sequentialGuidGenerator.NewId().ToString()),
			cancellationToken: cancellationToken);
	}
}