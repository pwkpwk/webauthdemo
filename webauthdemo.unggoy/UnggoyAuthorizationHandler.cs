using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace webauthdemo.unggoy;

/// <summary>
/// Handler that verifies the requiremets of the 'Unggoy' authorization policy
/// </summary>
internal sealed class UnggoyAuthorizationHandler(
    ILogger<UnggoyAuthorizationHandler> logger,
    IUnggoyActionVerifier actionVerifier)
    : IAuthorizationHandler
{
    private static readonly EventId FailureEventId = new(1, "Failure");
    private static readonly EventId VerifyiingEventId = new(2, "Verifying");
    private static readonly EventId BadArgsEventId = new(3, "BadArgs");

    async Task IAuthorizationHandler.HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.HasFailed)
        {
            return;
        }
        
        var httpContext = context.Resource as HttpContext;
        
        if (httpContext is null) 
        {
            logger.LogError(FailureEventId, "HTTP context is not available");
            context.Fail();
            return;
        }

        var endpoint = httpContext.GetEndpoint();
        
        if (endpoint is null) 
        {
            logger.LogError(FailureEventId, "HTTP endpoint is not available");
            context.Fail();
            return;
        }

        bool isTokenRequired = false;
        string? actionName = null;
        string? token = null;
        
        foreach (var requirement in context.PendingRequirements)
        {
            if (requirement is UnggoyTokenRequired tokenRequired) 
            {
                if (tokenRequired.IsRequired) 
                {
                    isTokenRequired = true;
                    token = tokenRequired.ExtractToken(httpContext);
                }
                
                if (isTokenRequired && token is null) 
                {
                    logger.LogError(FailureEventId, "Token is not available");
                    context.Fail(new AuthorizationFailureReason(this, "Token is not available"));
                    break;
                }
                context.Succeed(requirement);
            }
            else if (requirement is UnggoyActionNameRequired actionNameRequired)
            {
                actionName = actionNameRequired.ExtractActionName(endpoint);
                
                if (actionName is null) 
                {
                    logger.LogError(FailureEventId, "Action name is not available");
                    context.Fail(new AuthorizationFailureReason(this, "Action name is not available"));
                    break;
                }
                context.Succeed(requirement);
            }
        }
        
        if (!context.HasFailed && isTokenRequired) 
        {
            var valid = await VerifyTokenAsync(actionName, token, httpContext.RequestAborted);
            
            if (!valid) 
            {
                logger.LogError(FailureEventId, "Token validation has failed");
                context.Fail(new AuthorizationFailureReason(this, "Token validation has failed"));
            }
        }
    }
    
    private async Task<bool> VerifyTokenAsync(string? actionName, string? token, CancellationToken cancellation) 
    {
        logger.LogInformation(VerifyiingEventId, "Verifying token '{Token}' for '{Action}'", token, actionName);
        
        if (actionName is null || token is null) 
        {
            logger.LogError(BadArgsEventId, "Action name or Unggoy token is missing");
            return false;
        }
        
        return await actionVerifier.VerifyTokenAsync(actionName, token, cancellation);
    }
}