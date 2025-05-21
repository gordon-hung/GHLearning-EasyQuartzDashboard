namespace GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.Models.Entities;

public record JobEntity(
    string JobName,
    string JobDesc,
    Type JobType,
    string JobCronExpression,
    string JobCronExpressionDes);