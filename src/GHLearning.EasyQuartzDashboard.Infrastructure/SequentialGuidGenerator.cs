using GHLearning.EasyQuartzDashboard.SharedKernel;

namespace GHLearning.EasyQuartzDashboard.Infrastructure;

internal sealed class SequentialGuidGenerator : ISequentialGuidGenerator
{
	public Guid NewId() => SequentialGuid.SequentialGuidGenerator.Instance.NewGuid();
}