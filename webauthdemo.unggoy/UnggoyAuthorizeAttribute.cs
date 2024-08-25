using Microsoft.AspNetCore.Authorization;

namespace webauthdemo.unggoy;

/// <summary>
/// Attribute that enables authorization with the "Unggoy" policy.
/// </summary>
/// <remarks>
/// <para>ASP.NET is smart enough to understand attribute base classes.</para>
/// <para>The attribute may be applied to controller classes and action methods. When applied to a class, the attribute
/// enables ASP.NET authorization for all action method of the class.</para>
/// </remarks>
[Serializable]
public sealed class UnggoyAuthorizeAttribute() : AuthorizeAttribute(UnggoyExtensions.PolicyName);