using GHLearning.EasyQuartzDashboard.Application.Customers.Add;
using GHLearning.EasyQuartzDashboard.Core.Customers;
using GHLearning.EasyQuartzDashboard.Core.Customers.Models;
using GHLearning.EasyQuartzDashboard.SharedKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace GHLearning.EasyQuartzDashboard.ApplicationTest.Customers.Add;

public class CustomerAddRequestHandlerTest
{
	[Fact]
	public async Task Handle()
	{
		var fakeLogger = NullLoggerFactory.Instance.CreateLogger<CustomerAddRequestHandler>();
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeSequentialGuidGenerator = Substitute.For<ISequentialGuidGenerator>();
		var fakeCustomerRepository = Substitute.For<ICustomerRepository>();

		var request = new CustomerAddRequest(
			Name: "name");

		var order = Guid.NewGuid();
		_ = fakeSequentialGuidGenerator.NewId().Returns(order);

		var sut = new CustomerAddRequestHandler(
			fakeLogger,
			fakeTimeProvider,
			fakeSequentialGuidGenerator,
			fakeCustomerRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		await sut.Handle(request, cancellationTokenSource.Token);

		_ = fakeCustomerRepository
			.Received()
			.AddAsync(
			parameter: Arg.Is<CustomerAddParameter>(compare =>
			compare.Name == request.Name &&
			compare.Order == order.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}