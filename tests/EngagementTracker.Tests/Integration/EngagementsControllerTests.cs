using System.Net;
using System.Net.Http.Json;
using EngagementTracker.Core.Dtos;

namespace EngagementTracker.Tests.Integration;

/// <summary>
/// Integration tests for the Engagements endpoints.
/// Verifies role-based filtering, CRUD operations, and dashboard data.
/// </summary>
public class EngagementsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EngagementsControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetEngagements_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/engagements");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEngagements_AsPartner_ReturnsAllEngagements()
    {
        await AuthenticateAs("carol@example.com");
        var response = await _client.GetAsync("/api/engagements");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var engagements = await response.Content.ReadFromJsonAsync<List<EngagementSummaryDto>>();
        Assert.NotNull(engagements);
        Assert.Equal(3, engagements.Count);
    }

    [Fact]
    public async Task GetEngagements_AsAssociate_ReturnsOnlyAssignedEngagements()
    {
        await AuthenticateAs("alice@example.com");
        var response = await _client.GetAsync("/api/engagements");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var engagements = await response.Content.ReadFromJsonAsync<List<EngagementSummaryDto>>();
        Assert.NotNull(engagements);
        // Alice has time on eng1 and eng2, not eng3
        Assert.Equal(2, engagements.Count);
    }

    [Fact]
    public async Task GetEngagements_FilterByStatus_ReturnsFilteredResults()
    {
        await AuthenticateAs("carol@example.com");
        var response = await _client.GetAsync("/api/engagements?Status=Completed");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var engagements = await response.Content.ReadFromJsonAsync<List<EngagementSummaryDto>>();
        Assert.NotNull(engagements);
        Assert.Single(engagements);
        Assert.All(engagements, e => Assert.Equal("Completed", e.Status));
    }

    [Fact]
    public async Task GetDashboard_AsManager_ReturnsDashboardData()
    {
        await AuthenticateAs("bob@example.com");
        var response = await _client.GetAsync("/api/engagements/dashboard");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>();
        Assert.NotNull(dashboard);
        Assert.True(dashboard.TotalEngagements > 0);
        Assert.True(dashboard.TotalHoursLogged > 0);
    }

    [Fact]
    public async Task GetEngagementDetail_AsPartner_ReturnsDetail()
    {
        await AuthenticateAs("carol@example.com");

        var listResponse = await _client.GetAsync("/api/engagements");
        var engagements = await listResponse.Content.ReadFromJsonAsync<List<EngagementSummaryDto>>();
        int engagementId = engagements!.First().Id;

        var response = await _client.GetAsync($"/api/engagements/{engagementId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var detail = await response.Content.ReadFromJsonAsync<EngagementDetailDto>();
        Assert.NotNull(detail);
        Assert.Equal(engagementId, detail.Id);
        Assert.True(detail.BudgetHours > 0);
    }

    private async Task AuthenticateAs(string email)
    {
        var loginRequest = new LoginRequestDto { Email = email, Password = "password123" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);
    }
}
