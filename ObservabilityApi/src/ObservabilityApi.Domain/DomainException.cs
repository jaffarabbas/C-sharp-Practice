namespace ObservabilityApi.Domain;

/// <summary>
/// Raised when an operation would violate a domain invariant. Request validation should catch
/// these cases first; reaching this exception indicates a validation gap, and it is reported as 400.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
