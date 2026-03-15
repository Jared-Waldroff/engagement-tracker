using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Exceptions;
using EngagementTracker.Core.Interfaces;
using EngagementTracker.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace EngagementTracker.Tests.Unit;

/// <summary>
/// Tests the TimeEntryService business logic.
/// Verifies engagement existence validation and proper delegation to the repository.
/// </summary>
public class TimeEntryServiceTests
{
    private readonly Mock<ITimeEntryRepository> _repositoryMock;
    private readonly Mock<ILogger<TimeEntryService>> _loggerMock;
    private readonly TimeEntryService _service;

    public TimeEntryServiceTests()
    {
        _repositoryMock = new Mock<ITimeEntryRepository>();
        _loggerMock = new Mock<ILogger<TimeEntryService>>();
        _service = new TimeEntryService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateTimeEntry_WithValidEngagement_Succeeds()
    {
        var dto = new CreateTimeEntryDto
        {
            EngagementId = 1,
            Hours = 8,
            Date = DateTime.UtcNow.Date,
            Description = "Audit fieldwork"
        };

        var expectedEntry = new TimeEntryDto(
            1, 1, "Annual Audit", "Alice Chen", 8, dto.Date, "Audit fieldwork");

        _repositoryMock.Setup(r => r.EngagementExistsAsync(1)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.CreateAsync(dto, 1)).ReturnsAsync(expectedEntry);

        var result = await _service.CreateTimeEntryAsync(dto, 1);

        Assert.Equal(8, result.Hours);
        Assert.Equal("Alice Chen", result.UserName);
    }

    [Fact]
    public async Task CreateTimeEntry_WithNonExistentEngagement_ThrowsNotFoundException()
    {
        var dto = new CreateTimeEntryDto
        {
            EngagementId = 999,
            Hours = 4,
            Date = DateTime.UtcNow.Date,
            Description = "Testing"
        };

        _repositoryMock.Setup(r => r.EngagementExistsAsync(999)).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateTimeEntryAsync(dto, 1));
    }
}
