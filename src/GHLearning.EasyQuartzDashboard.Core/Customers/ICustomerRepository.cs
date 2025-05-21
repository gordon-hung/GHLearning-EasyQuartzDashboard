using GHLearning.EasyQuartzDashboard.Core.Customers.Models;

namespace GHLearning.EasyQuartzDashboard.Core.Customers;

public interface ICustomerRepository
{
	Task AddAsync(CustomerAddParameter parameter, CancellationToken cancellationToken = default);
}