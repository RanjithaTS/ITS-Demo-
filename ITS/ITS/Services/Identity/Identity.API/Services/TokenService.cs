using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager, ILogger<TokenService> logger)
    {
        _userManager = userManager;
        _logger = logger;
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
    }

    public async Task<string> CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.DisplayName)
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            //Expires = DateTime.Now.AddMinutes(5), // testing
            Expires = DateTime.Now.AddDays(Convert.ToInt32(_config["Jwt:ExpiryInDays"])),
            SigningCredentials = creds,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
    {
        if (expires != null)
        {
            return expires > DateTime.UtcNow;
        }
        return false;
    }

    public async Task<bool> ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = _key,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                // set clockskew to zero so tokens expire exactly at token expiration time
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var email = jwtToken.Claims.First(x => x.Type == "email").Value;

            // additional checks against user database:

            // does user with this email address exist?
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return false;

            // check if user is locked out
            var userLockedOut = await _userManager.IsLockedOutAsync(user);
            if (userLockedOut) return false;

            // return true if validation and additional checks are successful
            return true;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "Token Validation Exception: {message}", ex.Message);
            return false;
        }
        catch
        {
            // return false if validation fails
            return false;
        }
    }
}
