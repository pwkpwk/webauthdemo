using Microsoft.AspNetCore.Authorization;

namespace webauthdemo.unggoy;

/// <summary>
/// Attribute that enables authorization with the "Unggoy" policy.
/// </summary>
/// <remarks>
/// ASP.NET is smart enough to understand attribute base classes.
/// </remarks>
public sealed class UnggoyAuthorizeAttribute() : AuthorizeAttribute("Unggoy");
