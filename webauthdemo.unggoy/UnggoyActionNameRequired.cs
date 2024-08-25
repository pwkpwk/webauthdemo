using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace webauthdemo.unggoy;

/// <summary>
/// 'Unggoy' authorization policy requirement of presence of the action name (<see cref="UnggoyActionAttribute"/>) attribute.
/// </summary>
internal sealed record UnggoyActionNameRequired : IAuthorizationRequirement
{
    /// <summary>
    /// Helper called by the authorization handler to extract the action name applied to the action method
    /// of an API controller.
    /// </summary>
    /// <param name="endpoint">Endpoint object from the ASP.NET request processing pipeline.</param>
    /// <returns>Action name specified in the <see cref="UnggoyActionAttribute"/> attribute applied
    /// to the action method.</returns>
    public string? ExtractActionName(Endpoint endpoint)
    {
        var name = endpoint.Metadata.GetMetadata<UnggoyActionAttribute>()?.Name;
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }
}