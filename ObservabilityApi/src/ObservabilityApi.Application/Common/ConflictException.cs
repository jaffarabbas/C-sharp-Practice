namespace ObservabilityApi.Application.Common;

/// <summary>The request conflicts with the current state of a resource (duplicate key, stale version). Maps to 409.</summary>
public sealed class ConflictException(string message, Exception? innerException = null)
    : Exception(message, innerException);
