using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using webauthdemo.unggoy;

namespace webauthdemo.service.gRPC;

public class GrpcEchoService(ILogger<GrpcEchoService> logger) : EchoService.EchoServiceBase
{
    private static readonly EventId YellEventId = new EventId(1, "Yell");
    private static readonly EventId WhisperEventId = new EventId(2, "Whisper");
    
    [Authorize("Unggoy"), UnggoyAction("Yell")]
    public override Task<Utterance> Yell(Utterance request, ServerCallContext context)
    {
        logger.LogInformation(YellEventId, "Heard '{Utterance}'", request.Text);
        return Task.FromResult(new Utterance { Text = $"{request.Text}!" });
    }

    public override Task<Utterance> Whisper(Utterance request, ServerCallContext context)
    {
        logger.LogInformation(WhisperEventId, "Barely heard '{Utterance}'", request.Text);
        return Task.FromResult(new Utterance { Text = request.Text });
    }
}