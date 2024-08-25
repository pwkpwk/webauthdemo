using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace webauthdemo.unggoy;

internal sealed record UnggoyActionNameRequired : IAuthorizationRequirement 
{
    public string? ExtractActionName(Endpoint endpoint) 
    {
        var name = endpoint.Metadata.GetMetadata<UnggoyActionAttribute>()?.Name;
        
        if (name is null)
        {
            name = endpoint.DisplayName;
        }

        return name;
    }
}