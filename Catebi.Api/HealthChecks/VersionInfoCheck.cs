using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Catebi.Api.HealthChecks;

public class VersionInfoCheck(IConfiguration config) : IHealthCheck
{
    private readonly IConfiguration config = config;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var data = new Dictionary<string, object>
        {
            { "Version", config["VersionInfo:Version"]! },
            { "Build"  , config["VersionInfo:Build"]! }
        };
        var result = new HealthCheckResult(HealthStatus.Healthy, "Application version", data: data);

        return await Task.FromResult(result);
    }
}
