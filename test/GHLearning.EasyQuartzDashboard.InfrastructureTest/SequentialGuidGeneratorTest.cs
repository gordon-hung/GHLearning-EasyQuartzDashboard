using GHLearning.EasyQuartzDashboard.Infrastructure;

namespace GHLearning.EasyQuartzDashboard.InfrastructureTest;

public class SequentialGuidGeneratorTest
{
	[Fact]
	public void NewId()
	{
		var sut = new SequentialGuidGenerator();

		var actual = sut.NewId();

		Assert.NotNull(actual);
	}
}