using GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.Models;
using GHLearning.EasyQuartzDashboard.Web.Hubs;
using GHLearning.EasyQuartzDashboard.Web.JobHandlers;
using GHLearning.EasyQuartzDashboard.Web.Listeners;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddInfrastructure((services, options) => options.UseMongoDB(
    connectionString: builder.Configuration.GetConnectionString("MongoDb")!,
    databaseName: "easy"));
builder.Services.AddApplication();

//向DI容器註冊Job
builder.Services
    .AddSingleton<IJobListener, QuartzJobListener>()
    .AddSingleton<ISchedulerListener, QuartzSchedulerListener>();

//向DI容器註冊Host服務
builder.Services.AddSingleton<GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.QuartzHostedService>();
builder.Services.AddHostedService(sp => sp.GetService<GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.QuartzHostedService>()
?? throw new InvalidOperationException(nameof(GHLearning.EasyQuartzDashboard.Infrastructure.QuartzJob.QuartzHostedService)));

//向DI容器註冊JobSchedule
builder.Services.AddSingleton<CronJobHandler>();
builder.Services.AddSingleton(new JobCreation(
    JobName: "111",
    JobType: typeof(CronJobHandler),
    JobDesc: "測試Job11",
    JobCronExpression: "*/30 * * * * ?",
    JobCronExpressionDes: "每 30 秒"));
builder.Services.AddSingleton(new JobCreation(
    JobName: "222",
    JobType: typeof(CronJobHandler),
    JobDesc: "測試Job222",
    JobCronExpression: "*/52 * * * * ?",
    JobCronExpressionDes: "每 52 秒"));
builder.Services.AddSingleton(new JobCreation(
    JobName: "333",
    JobType: Type.GetType("GHLearning.EasyQuartzDashboard.Web.JobHandlers.CronJobHandler")!,
    JobDesc: "測試Job333",
    JobCronExpression: "*/10 * * * * ?",
    JobCronExpressionDes: "每 10 秒"));

// 設定 SignalR 服務
builder.Services
    .AddSignalR()
    .Services
    .AddSingleton<SchedulerHub>();

//Learn more about configuring OpenTelemetry at https://learn.microsoft.com/zh-tw/dotnet/core/diagnostics/observability-with-otel
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: builder.Configuration["ServiceName"]!.ToLower(),
            serviceNamespace: builder.Configuration["ServiceNamespace"]!,
            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"))
    .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(builder.Configuration["OtlpEndpointUrl"]!))
    .WithMetrics(metrics => metrics
        .AddMeter("GHLearning.")
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddQuartzInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation(options => options.Filter = (httpContext) =>
                !httpContext.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/live", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/metrics", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/favicon.ico", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.Value!.Equals("/api/events/raw", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.Value!.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/_vs", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/css", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/lib", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase) &&
                !httpContext.Request.Path.StartsWithSegments("/scalar", StringComparison.OrdinalIgnoreCase)));

//Learn more about configuring HealthChecks at https://learn.microsoft.com/zh-tw/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-9.0
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<SchedulerHub>("/schedulerHub");

app.UseHealthChecks("/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});
app.UseHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => true,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.Run();