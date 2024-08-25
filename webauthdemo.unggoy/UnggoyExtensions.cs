using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace webauthdemo.unggoy;

public static class UnggoyExtensions
{
    /// <summary>
    /// Name of the authorization policy shared between <see cref="AuthorizeAttribute.Policy"/> property
    /// and the call of <see cref="AuthorizationOptions.AddPolicy(string,Microsoft.AspNetCore.Authorization.AuthorizationPolicy)"/>
    /// </summary>
    internal const string PolicyName = "Unggoy";

    public static IServiceCollection AddUnggoyAuthorization(this IServiceCollection services) => services
        .AddAuthorization(AddUnggoyAuthorizationOptions)
        .AddSingleton<IAuthorizationHandler, UnggoyAuthorizationHandler>();

    private static void AddUnggoyAuthorizationOptions(AuthorizationOptions options)
    {
        // 'Unggoy' authorization policy has UnggoyActionNameRequired and UnggoyTokenRequired requirements
        //
        // Setting up the policy with 2 requirements is for illustration purposes only.
        // The action name and token requirements are not independent and in practice must be merged
        // into 1 requirement for that the authorization handler shall extract the token and action name
        // from the HTTP context.
        options.AddPolicy(
            PolicyName,
            policy => policy.AddRequirements(new UnggoyActionNameRequired(), new UnggoyTokenRequired()));
    }
}