using webauthdemo.unggoy;

namespace webauthdemo.service.Authorization;

public class UnggoyActionVerifier(ILogger<UnggoyActionVerifier> logger) : IUnggoyActionVerifier
{
    private static readonly EventId VerifyingEventId = new(1, "Verifying");
    private static readonly EventId FailedEventId = new(2, "Failed");
    private static readonly EventId VerifiedEventId = new(3, "Verified");

    async Task<bool> IUnggoyActionVerifier.VerifyTokenAsync(string action, string token, CancellationToken cancellation)
    {
        logger.LogInformation(VerifyingEventId, "Verifying token '{token}' for '{Action}'", token, action);
        await Task.Delay(10, cancellation);
        
        // Mock token verification - check that the token string contsins 'unggoy', so we can show that invalid
        // tokens cause call failures
        bool verified = token.Contains("unggoy", StringComparison.OrdinalIgnoreCase);

        if (!verified)
        {
            logger.LogError(FailedEventId, "Failed to verify token '{Token}' for '{Action}'", token, action);
        }
        else
        {
            logger.LogInformation(VerifiedEventId, "Verified token '{Token}' for '{Action}'", token, action);
        }

        return verified;
    }
}