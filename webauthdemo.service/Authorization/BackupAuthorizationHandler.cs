using Microsoft.AspNetCore.Authorization;

namespace webauthdemo.service.Authorization;

public class BackupAuthorizationHandler(ILogger<BackupAuthorizationHandler> logger)
    : IAuthorizationHandler
{
    private static readonly EventId HandleEventId = new(1, "Handle");

    Task IAuthorizationHandler.HandleAsync(AuthorizationHandlerContext context)
    {
        logger.LogInformation(HandleEventId,
            "Additional authorization handler | HasFailed={Failed}",
            context.HasFailed);
        return Task.CompletedTask;
    }
}