namespace EngagementTracker.Core.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist in the system.
/// Maps to HTTP 404 Not Found in the global exception middleware.
/// </summary>
public class NotFoundException : Exception
{
    public string ErrorCode { get; }

    public NotFoundException(string message, string errorCode = "RESOURCE_NOT_FOUND")
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
