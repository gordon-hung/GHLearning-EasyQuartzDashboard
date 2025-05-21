using GHLearning.EasyQuartzDashboard.Core.Customers.Models;
using GHLearning.EasyQuartzDashboard.Infrastructure.Customers;
using GHLearning.EasyQuartzDashboard.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace GHLearning.EasyQuartzDashboard.InfrastructureTest.Customers;

public class CustomerRepositoryTest
{
	[Fact]
	public async Task Add()
	{
		var options = new DbContextOptionsBuilder<EasyDbContext>()
			.UseInMemoryDatabase(databaseName: $"dbo.{nameof(Add)}")
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		var context = new EasyDbContext(options);
		_ = context.Database.EnsureDeleted();
		_ = context.Database.EnsureCreated();

		var fakeTimeProvider = Substitute.For<TimeProvider>();

		var parameter = new CustomerAddParameter(
			Name: "name",
			Order: "order");

		var sut = new CustomerRepository(
			context,
			fakeTimeProvider);

		await sut.AddAsync(parameter);

		var actual = await context.Customers
		.Where(x => x.Order == parameter.Order)
		.SingleAsync();

		Assert.NotNull(actual);
		Assert.Equal(parameter.Name, actual.Name);
	}
}