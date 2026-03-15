using System.Net;
using System.Net.Http.Json;
using EngagementTracker.Core.Dtos;

namespace EngagementTracker.Tests.Integration;

/// <summary>
/// Integration tests for the Auth endpoints.
/// Tests the full HTTP pipeline: routing → controller → service → repository → database.
/// </summary>
public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "alice@example.com",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginResponse);
        Assert.False(string.IsNullOrEmpty(loginResponse.AccessToken));
        Assert.Equal("Bearer", loginResponse.TokenType);
        Assert.Equal("alice@example.com", loginResponse.User.Email);
        Assert.Equal("Associate", loginResponse.User.Role);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "alice@example.com",
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns401()
    {
        var loginRequest = new LoginRequestDto
        {
            Email = "nobody@example.com",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/auth/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ReturnsProfile()
    {
        // Login first to get a token
        var loginRequest = new LoginRequestDto
        {
            Email = "bob@example.com",
            Password = "password123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Use the token to get the profile
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);

        var profileResponse = await _client.GetAsync("/api/auth/profile");

        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);

        var profile = await profileResponse.Content.ReadFromJsonAsync<UserProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal("bob@example.com", profile.Email);
        Assert.Equal("Manager", profile.Role);
    }
}
