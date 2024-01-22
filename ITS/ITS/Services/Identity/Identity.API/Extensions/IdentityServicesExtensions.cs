using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.API.Extensions;

public static class IdentityServicesExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<IdentityRole>>()
            .AddEntityFrameworkStores<IdentityDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero,
                    ValidateActor = true,
                };
            });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
            opt.AddPolicy(Policies.IsMarketing, Policies.IsMarketingPolicy());
            opt.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
        });

        return services;
    }
}
