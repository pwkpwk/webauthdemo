using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace webauthdemo.unggoy;

public static class UnggoyExtensions
{
    public static IServiceCollection AddUnggoyAuthorization(this IServiceCollection services) => services
        .AddAuthorization(AddUnggoyAuthorizationOptions)
        .AddSingleton<IAuthorizationHandler, UnggoyAuthorizationHandler>();

    private static void AddUnggoyAuthorizationOptions(AuthorizationOptions options)
    {
        // 'Unggoy' authorization policy has UnggoyActionNameRequired and UnggoyTokenRequired requirements
        options.AddPolicy(
            "Unggoy",
            policy => policy.AddRequirements(
                new UnggoyActionNameRequired(),
                new UnggoyTokenRequired(IsRequired: true)));
    }
}