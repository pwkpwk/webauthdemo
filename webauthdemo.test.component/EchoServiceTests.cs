using Grpc.Core;
using Grpc.Net.Client;

namespace webauthdemo.test.component;

[TestFixture]
public class EchoServiceTests
{
    private GrpcChannel _authorizedChannel;
    private GrpcChannel _unauthorizedChannel;
    private GrpcChannel _clearChannel;
    private EchoService.EchoServiceClient _authorizedEchoClient;
    private EchoService.EchoServiceClient _unauthorizedEchoClient;
    private EchoService.EchoServiceClient _clearEchoClient;
    
    [SetUp]
    public void SetUpTest()
    {
        string? baseUri = TestContext.Parameters["httpsUri"]; 
        CallCredentials validCredentials = CallCredentials.FromInterceptor(async (_, metadata) =>
        {
            metadata.Add("Authorization", $"unggoy test-unggoy-token");
        });
        CallCredentials invalidCredentials = CallCredentials.FromInterceptor(async (_, metadata) =>
        {
            metadata.Add("Authorization", $"bearer test-unggoy-token");
        });
        _authorizedChannel = GrpcChannel.ForAddress(
            baseUri,
            new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), validCredentials)
            });
        _unauthorizedChannel = GrpcChannel.ForAddress(
            baseUri,
            new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), invalidCredentials)
            });
        _clearChannel = GrpcChannel.ForAddress(baseUri);
        _authorizedEchoClient = new EchoService.EchoServiceClient(_authorizedChannel);
        _unauthorizedEchoClient = new EchoService.EchoServiceClient(_unauthorizedChannel);
        _clearEchoClient = new EchoService.EchoServiceClient(_clearChannel);
    }

    [TearDown]
    public void TearDownTest()
    {
        _authorizedChannel.Dispose();
        _unauthorizedChannel.Dispose();
        _clearChannel.Dispose();
    }
    
    [Test]
    public async Task ClearChannel_Whisper_CorrectEcho()
    {
        var echo = await _clearEchoClient.WhisperAsync(
            new Utterance { Text = "Hey" },
            cancellationToken: CancellationToken.None);
        
        Assert.That(echo.Text, Is.EqualTo("Hey"));
    }
    
    [Test]
    public async Task UnauthorizedChannel_Whisper_CorrectEcho()
    {
        var echo = await _unauthorizedEchoClient.WhisperAsync(
            new Utterance { Text = "Hey" },
            cancellationToken: CancellationToken.None);
        
        Assert.That(echo.Text, Is.EqualTo("Hey"));
    }
    
    [Test]
    public async Task AuthorizedChannel_Whisper_CorrectEcho()
    {
        var echo = await _authorizedEchoClient.WhisperAsync(
            new Utterance { Text = "Hey" },
            cancellationToken: CancellationToken.None);
        
        Assert.That(echo.Text, Is.EqualTo("Hey"));
    }
    
    [Test]
    public async Task AuthorizedChannel_Yell_CorrectEcho()
    {
        var echo = await _authorizedEchoClient.YellAsync(
            new Utterance { Text = "Hey" },
            cancellationToken: CancellationToken.None);
        
        Assert.That(echo.Text, Is.EqualTo("Hey!"));
    }
    
    [Test]
    public void UnauthorizedChannel_Yell_Throws()
    {
        var exception = Assert.ThrowsAsync<RpcException>(async () => 
        {
            await _unauthorizedEchoClient.YellAsync(
                new Utterance { Text = "Hey" },
                cancellationToken: CancellationToken.None);
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
            Assert.That(exception.Status.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
        });
    }
    
    [Test]
    public void ClearChannel_Yell_Throws()
    {
        var exception = Assert.ThrowsAsync<RpcException>(async () => 
        {
            await _clearEchoClient.YellAsync(
                new Utterance { Text = "Hey" },
                cancellationToken: CancellationToken.None);
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
            Assert.That(exception.Status.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
        });
    }
}