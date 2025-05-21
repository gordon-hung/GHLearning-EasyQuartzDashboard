using MediatR;

namespace GHLearning.EasyQuartzDashboard.Application.Customers.Add;

public record CustomerAddRequest(
    string Name) : IRequest;