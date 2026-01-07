using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityAndAccessManagement.Api.Auth;

public static class KeycloakExtensions
{
    public static IServiceCollection AddKeycloakJwt(
        this IServiceCollection services,
        IConfiguration config)
    {
        var authority = config["Keycloak:Authority"]
            ?? throw new InvalidOperationException("Missing Keycloak Authority");

        var audience = config["Keycloak:Audience"]
            ?? throw new InvalidOperationException("Missing Keycloak Audience");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authority,
                    ValidAudience = audience
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Farmer", policy => policy.RequireRole("farmer"));
            options.AddPolicy("Driver", policy => policy.RequireRole("driver"));
            options.AddPolicy("Coordinator", policy => policy.RequireRole("coordinator"));
            options.AddPolicy("Buyer", policy => policy.RequireRole("buyer"));
        });

        return services;
    }
}
