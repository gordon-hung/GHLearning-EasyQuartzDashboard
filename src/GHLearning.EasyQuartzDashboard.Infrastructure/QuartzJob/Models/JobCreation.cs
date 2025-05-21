namespace GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.Models;

public record JobCreation(
    string JobName,
    string JobDesc,
    Type JobType,
    string JobCronExpression,
    string JobCronExpressionDes);