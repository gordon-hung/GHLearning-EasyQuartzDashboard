using System.ComponentModel;

namespace GHLearning.EasyQuartzDashboard.Web.Extension;

public static class EnumExtensions
{
	public static string GetDescription(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
							  .FirstOrDefault() as DescriptionAttribute;
		return attribute?.Description ?? value.ToString();
	}
}