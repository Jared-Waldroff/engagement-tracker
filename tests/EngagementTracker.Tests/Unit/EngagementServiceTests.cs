using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;
using EngagementTracker.Core.Exceptions;
using EngagementTracker.Core.Interfaces;
using EngagementTracker.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace EngagementTracker.Tests.Unit;

/// <summary>
/// Tests the EngagementService business logic. Uses mocked repositories
/// to verify role-based access control and error handling.
/// </summary>
public class EngagementServiceTests
{
    private readonly Mock<IEngagementRepository> _repositoryMock;
    private readonly Mock<ILogger<EngagementService>> _loggerMock;
    private readonly EngagementService _service;

    public EngagementServiceTests()
    {
        _repositoryMock = new Mock<IEngagementRepository>();
        _loggerMock = new Mock<ILogger<EngagementService>>();
        _service = new EngagementService(_repositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Verifies that GetEngagementsAsync passes the correct role and filters to the repository.
    /// </summary>
    [Fact]
    public async Task GetEngagements_PassesRoleAndFiltersToRepository()
    {
        var filters = new EngagementFilterDto { Status = "Active" };
        _repositoryMock
            .Setup(r => r.GetEngagementsAsync(1, UserRole.Associate, filters))
            .ReturnsAsync(new List<EngagementSummaryDto>());

        var result = await _service.GetEngagementsAsync(1, UserRole.Associate, filters);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.GetEngagementsAsync(1, UserRole.Associate, filters), Times.Once);
    }

    /// <summary>
    /// Verifies that requesting a non-existent engagement throws NotFoundException.
    /// </summary>
    [Fact]
    public async Task GetEngagementDetail_WhenNotFound_ThrowsNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.UserHasAccessAsync(999, 1, UserRole.Partner))
            .ReturnsAsync(true);
        _repositoryMock
            .Setup(r => r.GetEngagementDetailAsync(999))
            .ReturnsAsync((EngagementDetailDto?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetEngagementDetailAsync(999, 1, UserRole.Partner));
    }

    /// <summary>
    /// Verifies that accessing an engagement without permission throws ForbiddenException.
    /// The engagement exists but the user has no access.
    /// </summary>
    [Fact]
    public async Task GetEngagementDetail_WhenUnauthorized_ThrowsForbiddenException()
    {
        _repositoryMock
            .Setup(r => r.UserHasAccessAsync(1, 5, UserRole.Associate))
            .ReturnsAsync(false);
        _repositoryMock
            .Setup(r => r.GetEngagementDetailAsync(1))
            .ReturnsAsync(CreateSampleDetail());

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.GetEngagementDetailAsync(1, 5, UserRole.Associate));
    }

    /// <summary>
    /// Verifies that Associates cannot update engagements even if they have access.
    /// </summary>
    [Fact]
    public async Task UpdateEngagement_AsAssociate_ThrowsForbiddenException()
    {
        _repositoryMock
            .Setup(r => r.UserHasAccessAsync(1, 1, UserRole.Associate))
            .ReturnsAsync(true);

        var dto = new UpdateEngagementDto { Title = "New Title" };

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.UpdateEngagementAsync(1, dto, 1, UserRole.Associate));
    }

    /// <summary>
    /// Verifies that Managers can update their managed engagements.
    /// </summary>
    [Fact]
    public async Task UpdateEngagement_AsManager_Succeeds()
    {
        var expectedSummary = CreateSampleSummary();
        _repositoryMock
            .Setup(r => r.UserHasAccessAsync(1, 2, UserRole.Manager))
            .ReturnsAsync(true);
        _repositoryMock
            .Setup(r => r.UpdateAsync(1, It.IsAny<UpdateEngagementDto>()))
            .ReturnsAsync(expectedSummary);

        var dto = new UpdateEngagementDto { Title = "Updated" };
        var result = await _service.UpdateEngagementAsync(1, dto, 2, UserRole.Manager);

        Assert.Equal(expectedSummary.Title, result.Title);
    }

    /// <summary>
    /// Verifies that Partners can see engagement details for any engagement.
    /// </summary>
    [Fact]
    public async Task GetEngagementDetail_AsPartner_ReturnsDetail()
    {
        var expected = CreateSampleDetail();
        _repositoryMock
            .Setup(r => r.UserHasAccessAsync(1, 3, UserRole.Partner))
            .ReturnsAsync(true);
        _repositoryMock
            .Setup(r => r.GetEngagementDetailAsync(1))
            .ReturnsAsync(expected);

        var result = await _service.GetEngagementDetailAsync(1, 3, UserRole.Partner);

        Assert.Equal(expected.Title, result.Title);
        Assert.Equal(expected.Id, result.Id);
    }

    private static EngagementSummaryDto CreateSampleSummary() =>
        new(1, "Test Client", "Test Engagement", "Active", 100, 50, 50.0m, "OnTrack", "Bob Martinez",
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), null);

    private static EngagementDetailDto CreateSampleDetail() =>
        new(1, "Test Client", "Technology", "Test Engagement", "Description", "Active",
            100, 175, 50, 50.0m, "OnTrack", 17500, 8750, "Bob Martinez",
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), null,
            new List<TimeEntryDto>(), new List<UserHoursSummaryDto>());
}
