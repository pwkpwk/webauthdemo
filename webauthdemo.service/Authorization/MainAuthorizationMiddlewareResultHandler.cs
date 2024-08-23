using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace webauthdemo.service.Authorization;

public class MainAuthorizationMiddlewareResultHandler(ILogger<MainAuthorizationMiddlewareResultHandler> logger)
    : IAuthorizationMiddlewareResultHandler
{
    private static readonly EventId UnauthorisedEventId = new EventId(1, "Unauthorised");
    
    Task IAuthorizationMiddlewareResultHandler.HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded)
        {
            logger.LogError(UnauthorisedEventId, "Unauthorized {Endpoint}", context.GetEndpoint());
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        
        return next(context);
    }
}