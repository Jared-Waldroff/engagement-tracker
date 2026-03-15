namespace EngagementTracker.Core.Exceptions;

/// <summary>
/// Thrown when a request fails business-level validation.
/// Maps to HTTP 400 Bad Request in the global exception middleware.
/// Carries field-level errors for structured error responses.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public string ErrorCode { get; }

    public ValidationException(string message, IDictionary<string, string[]>? errors = null, string errorCode = "VALIDATION_FAILED")
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
        ErrorCode = errorCode;
    }
}
