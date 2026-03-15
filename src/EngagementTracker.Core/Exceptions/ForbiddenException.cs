namespace EngagementTracker.Core.Exceptions;

/// <summary>
/// Thrown when the authenticated user does not have permission to access a resource.
/// Maps to HTTP 403 Forbidden in the global exception middleware.
/// </summary>
public class ForbiddenException : Exception
{
    public string ErrorCode { get; }

    public ForbiddenException(string message, string errorCode = "ACCESS_DENIED")
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
