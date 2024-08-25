namespace webauthdemo.unggoy;

public interface IUnggoyActionVerifier
{
    Task<bool> VerifyTokenAsync(string action, string token, CancellationToken cancellationToken);
}