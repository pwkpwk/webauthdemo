using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.HighPerformance;

namespace webauthdemo.unggoy;

/// <summary>
/// Combined authorization handler that extracts the token from headers and action name from metadata
/// and verifies the action with the token.
/// </summary>
/// <param name="logger">Logger to report results.</param>
/// <param name="actionVerifier">Action verifier component.</param>
/// <param name="contextAccessor">Accessor of the HTTP context.</param>
/// <remarks>
/// The <see cref="AuthorizationHandler{TRequirement}"/> base class is good only for the cases when there is only one
/// handler for each policy requirement in all policies, as it enumerates all requirements in the authorization context.
/// </remarks>
internal class UnggoyCombinedAuthorizationHandler(
    ILogger<UnggoyCombinedAuthorizationHandler> logger,
    IUnggoyActionVerifier actionVerifier,
    IHttpContextAccessor contextAccessor)
    : AuthorizationHandler<UnggoyTokenRequired>
{
    private static readonly EventId NoActionEventId = new(1, "NoAction");
    private static readonly EventId NoTokenEventId = new(2, "NoToken");
    private static readonly EventId VerifiedEventId = new(3, "Verified");
    private static readonly EventId UnverifiedEventId = new(4, "Unverified");

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UnggoyTokenRequired requirement)
    {
        if (context.HasFailed)
        {
            return;
        }

        // The benefit of use of the HTTP context accessor is that for testing it can be injected separately
        // from the authorization context.
        var httpContext = contextAccessor.HttpContext;
        var endpoint = httpContext?.GetEndpoint();
        var actionRequired = endpoint?.Metadata?.GetMetadata<UnggoyActionAttribute>();

        if (actionRequired is null)
        {
            logger.LogError(NoActionEventId, "Action name is not available for '{Endpoint}'", endpoint?.DisplayName);
            context.Fail(new AuthorizationFailureReason(this, $"Action name is not available for '{endpoint}"));
            return;
        }

        if (!ExtractUnggoyToken(contextAccessor.HttpContext!.Request.Headers, out var token))
        {
            logger.LogError(NoTokenEventId, "Token is not available for '{Endpoint}'", endpoint?.DisplayName);
            context.Fail(new AuthorizationFailureReason(this, $"Token is not available for '{endpoint}"));
            return;
        }

        var verified = await actionVerifier.VerifyTokenAsync(actionRequired.Name, token, httpContext!.RequestAborted);

        if (verified)
        {
            logger.LogInformation(VerifiedEventId, "Action '{Action}' has been verified", actionRequired.Name);
            context.Succeed(requirement);
        }
        else
        {
            logger.LogError(UnverifiedEventId, "Failed to verify ction '{Action}'", actionRequired.Name);
            context.Fail(new AuthorizationFailureReason(this, $"Failed to verify action '{actionRequired.Name}'"));
        }
    }

    private bool ExtractUnggoyToken(IHeaderDictionary headers, out string token)
    {
        foreach (var header in headers.Authorization)
        {
            string? value = ExtractTokenFromString(header);

            if (value is not null)
            {
                token = value;
                return true;
            }
        }

        token = string.Empty;
        return false;
    }

    private static string? ExtractTokenFromString(string? auth)
    {
        if (string.IsNullOrEmpty(auth))
        {
            return null;
        }

        int index = 0;
        foreach (var token in auth.Tokenize(' '))
        {
            switch (index)
            {
                case 0:
                    if (token is "unggoy" or "Unggoy")
                    {
                        index = 1;
                        break;
                    }

                    return null;

                case 1:
                    return token.ToString();
            }
        }

        return null;
    }
}