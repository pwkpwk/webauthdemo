using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Toolkit.HighPerformance;

namespace webauthdemo.unggoy;

internal sealed record UnggoyTokenRequired(bool IsRequired) : IAuthorizationRequirement
{
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
                    index = token is "unggoy" ? 1 : 2;
                    break;

                case 1:
                    return token.ToString();
                    break;

                default:
                    continue;
            }
        }

        return null;
    }
}