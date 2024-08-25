using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Toolkit.HighPerformance;

namespace webauthdemo.unggoy;

/// <summary>
/// 'Unggoy' authorization policy requirement of the presence of Authorization HTTP header with 'unggoy' scheme
/// and a non-empty token.
/// </summary>
internal sealed record UnggoyTokenRequired : IAuthorizationRequirement
{
    /// <summary>
    /// <see cref="UnggoyAuthorizationHandler"/> calls this method to extract Unggoy token from the Authorization
    /// HTTP header.
    /// </summary>
    /// <param name="context">ASP.NET HTTP context.</param>
    /// <returns>Unggoy token string from the first Authorization header with 'unggoy' scheme, or null if none
    /// of the Authorization headers has a token.</returns>
    public string? ExtractToken(HttpContext context)
    {
        foreach (var header in context.Request.Headers.Authorization)
        {
            string? value = ExtractTokenFromString(header);

            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    private static string? ExtractTokenFromString(string? auth)
    {
        if (string.IsNullOrEmpty(auth))
        {
            return null;
        }

        int index = 0;
        foreach (var token in auth.Tokenize(' '))
        {
            switch (index)
            {
                case 0:
                    if (token is "unggoy" or "Unggoy")
                    {
                        index = 1;
                        break;
                    }

                    return null;

                case 1:
                    return token.ToString();
            }
        }

        return null;
    }
}