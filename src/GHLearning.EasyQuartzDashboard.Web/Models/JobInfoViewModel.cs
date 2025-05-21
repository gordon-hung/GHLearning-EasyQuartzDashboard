using GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.Models;

namespace GHLearning.EasyQuartzDashboard.Web.Models;

public record JobInfoViewModel(
    string JobName,
    string JobDesc,
    string JobType,
    string JobCronExpression,
    string JobCronExpressionDes,
    JobStatus JobStatus,
    string JobStatusName,
    DateTime? StartTime,
    DateTime? EndTime,
    DateTime? PreviousFireTime,
    DateTime? NextFireTime);