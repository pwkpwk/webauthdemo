using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace webauthdemo.unggoy;

/// <summary>
/// Handler that verifies the requiremets of the 'Unggoy' authorization policy
/// </summary>
/// <remarks>
/// The handler verifies 2 policy requirements for illustration purposes only.
/// <para>
/// In practice, both requirements must be merged into one, for that the handler shall extract the token from
/// the request headers and the action name from the metadata attribute, and verify if the token allows the action.
/// </para>
/// <para>
/// Instead of implementing the <see cref="IAuthorizationHandler"/> interface, the handler may inherit
/// <see cref="AuthorizationHandler{TRequirement}"/> that loops through all requirements, finds the one that matches
/// its generic argument, and calls a virtual method to verify that requirement only.
/// </para>
/// </remarks>
internal sealed class UnggoyAuthorizationHandler(
    ILogger<UnggoyAuthorizationHandler> logger,
    IUnggoyActionVerifier actionVerifier)
    : IAuthorizationHandler
{
    private static readonly EventId FailureEventId = new(1, "Failure");
    private static readonly EventId VerifyingEventId = new(2, "Verifying");
    private static readonly EventId BadArgsEventId = new(3, "BadArgs");

    async Task IAuthorizationHandler.HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.HasFailed)
        {
            return;
        }

        var httpContext = context.Resource as HttpContext;
        var endpoint = httpContext?.GetEndpoint();

        if (endpoint is null)
        {
            logger.LogError(FailureEventId, "HTTP endpoint is not available");
            return;
        }

        string? actionName = null;
        string? token = null;

        foreach (var requirement in context.PendingRequirements)
        {
            if (requirement is UnggoyTokenRequired tokenRequired)
            {
                token = tokenRequired.ExtractToken(httpContext);

                if (token is null)
                {
                    logger.LogError(FailureEventId, "Required Unggoy token is missing");
                    return;
                }

                context.Succeed(requirement);
            }
            else if (requirement is UnggoyActionNameRequired actionNameRequired)
            {
                actionName = actionNameRequired.ExtractActionName(endpoint);

                if (actionName is null)
                {
                    logger.LogError(FailureEventId, "Action name is not available");
                    return;
                }

                context.Succeed(requirement);
            }
        }

        var valid = await VerifyTokenAsync(actionName, token, httpContext.RequestAborted);

        if (!valid)
        {
            logger.LogError(FailureEventId, "Token validation has failed");
            context.Fail(new AuthorizationFailureReason(this, "Token validation has failed"));
        }
    }

    private async Task<bool> VerifyTokenAsync(string? actionName, string? token, CancellationToken cancellation)
    {
        logger.LogInformation(VerifyingEventId, "Verifying token '{Token}' for '{Action}'", token, actionName);

        if (actionName is null || token is null)
        {
            logger.LogError(BadArgsEventId, "Action name or Unggoy token is missing");
            return false;
        }

        return await actionVerifier.VerifyTokenAsync(actionName, token, cancellation);
    }
}