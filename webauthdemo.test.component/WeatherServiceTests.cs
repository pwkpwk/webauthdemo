using Grpc.Core;
using Grpc.Net.Client;

namespace webauthdemo.test.component;

[TestFixture]
public class WeatherServiceTests
{
    private GrpcChannel _authorizedChannel;
    private GrpcChannel _unauthorizedChannel;
    private WeatherService.WeatherServiceClient _authorizedClient;
    private WeatherService.WeatherServiceClient _unauthorizedClient;

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
        _authorizedClient = new WeatherService.WeatherServiceClient(_authorizedChannel);
        _unauthorizedChannel = GrpcChannel.ForAddress(
            baseUri,
            new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), invalidCredentials)
            });
        _unauthorizedClient = new WeatherService.WeatherServiceClient(_unauthorizedChannel);
    }

    [TearDown]
    public void TearDownTest()
    {
        _authorizedChannel.Dispose();
        _unauthorizedChannel.Dispose();
    }

    [Test]
    public async Task AuthorizedChannel_QueryAsync_CorrectResponse()
    {
        var reply = await _authorizedClient.QueryAsync(
            new WeatherRequest { Locale = "France" },
            cancellationToken: CancellationToken.None);
        
        Assert.That(reply.Description, Is.EqualTo("Weather in France is nice"));
    }

    [Test]
    public void UnauthorizedChannel_QueryAsync_Throws()
    {
        var exception = Assert.ThrowsAsync<RpcException>(async () =>
        {
            await _unauthorizedClient.QueryAsync(
                new WeatherRequest { Locale = "France" },
                cancellationToken: CancellationToken.None);
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.Status.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
            Assert.That(exception.StatusCode, Is.EqualTo(StatusCode.Unauthenticated));
        });
    }
}