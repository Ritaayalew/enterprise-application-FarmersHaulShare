using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace SharedKernel;

public class ClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = (ClaimsIdentity)principal.Identity!;

        if (!principal.HasClaim(c => c.Type == "realm_access")) return Task.FromResult(principal);

        var realmAccessClaim = claimsIdentity.FindFirst("realm_access");
        if (realmAccessClaim == null) return Task.FromResult(principal);

        var realmAccessJson = realmAccessClaim.Value;

        using var doc = JsonDocument.Parse(realmAccessJson);
        var rolesElement = doc.RootElement.GetProperty("roles");

        foreach (var role in rolesElement.EnumerateArray())
        {
            var roleValue = role.GetString();
            if (!string.IsNullOrEmpty(roleValue))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
            }
        }

        return Task.FromResult(principal);
    }
}