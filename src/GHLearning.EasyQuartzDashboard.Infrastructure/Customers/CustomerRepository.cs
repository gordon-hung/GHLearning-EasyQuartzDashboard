using GHLearning.EasyQuartzDashboard.Core.Customers;
using GHLearning.EasyQuartzDashboard.Core.Customers.Models;
using GHLearning.EasyQuartzDashboard.Infrastructure.Entities;

namespace GHLearning.EasyQuartzDashboard.Infrastructure.Customers;

internal class CustomerRepository(
	EasyDbContext context,
	TimeProvider timeProvider) : ICustomerRepository
{
	public async Task AddAsync(CustomerAddParameter parameter, CancellationToken cancellationToken = default)
	{
		var entity = new Entities.Models.Customer
		{
			Name = parameter.Name,
			Order = parameter.Order,
			CreatedAt = timeProvider.GetUtcNow()
		};

		await context.Customers.AddAsync(entity, cancellationToken).ConfigureAwait(false);
		await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
}