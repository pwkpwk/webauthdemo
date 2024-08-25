using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using webauthdemo.unggoy;

namespace webauthdemo.service.gRPC;

[Authorize("Unggoy")]
public class GrpcWeatherService(ILogger<GrpcWeatherService> logger) : WeatherService.WeatherServiceBase
{
    private static readonly EventId QueryEventId = new(1, "Query");
    
    [UnggoyAction("Query")]
    public override Task<WeatherReport> Query(WeatherRequest request, ServerCallContext context)
    {
        logger.LogInformation(QueryEventId, "Received query for '{Locale}", request.Locale);
        return Task.FromResult(new WeatherReport(){ Description = $"Weather in {request.Locale} is nice" });
    }
}