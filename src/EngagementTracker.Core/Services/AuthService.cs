using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EngagementTracker.Core.Services;

/// <summary>
/// Handles user authentication using BCrypt for password verification
/// and JWT for stateless session management.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        string token = GenerateJwtToken(user.Id, user.Email, user.Role.ToString());
        int expiresInSeconds = _configuration.GetValue("Jwt:ExpiresInMinutes", 60) * 60;

        _logger.LogInformation("User {UserId} ({Role}) logged in successfully", user.Id, user.Role);

        return new LoginResponseDto(
            AccessToken: token,
            TokenType: "Bearer",
            ExpiresIn: expiresInSeconds,
            User: new UserProfileDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString())
        );
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new Exceptions.NotFoundException($"User with ID {userId} not found.", "USER_NOT_FOUND");

        return new UserProfileDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString());
    }

    private string GenerateJwtToken(int userId, string email, string role)
    {
        string secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        int expiresInMinutes = _configuration.GetValue("Jwt:ExpiresInMinutes", 60);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "EngagementTracker",
            audience: _configuration["Jwt:Audience"] ?? "EngagementTracker",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
