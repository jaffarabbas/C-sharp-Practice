namespace ApiTemplate.Shared.Configuration
{
    public class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";

        public FixedPolicyOptions FixedPolicy { get; set; } = new();
        public SlidingPolicyOptions SlidingPolicy { get; set; } = new();
        public TokenPolicyOptions TokenPolicy { get; set; } = new();
        public ConcurrencyPolicyOptions ConcurrencyPolicy { get; set; } = new();
        public PerIPPolicyOptions PerIPPolicy { get; set; } = new();
        public PerUserPolicyOptions PerUserPolicy { get; set; } = new();
        public TieredPolicyOptions TieredPolicy { get; set; } = new();
        public GlobalLimiterOptions GlobalLimiter { get; set; } = new();
        public OnRejectedOptions OnRejected { get; set; } = new();
    }

    public class FixedPolicyOptions
    {
        public int PermitLimit { get; set; } = 10;
        public int WindowMinutes { get; set; } = 1;
        public int QueueLimit { get; set; } = 2;
    }

    public class SlidingPolicyOptions
    {
        public int PermitLimit { get; set; } = 100;
        public int WindowMinutes { get; set; } = 1;
        public int SegmentsPerWindow { get; set; } = 4;
        public int QueueLimit { get; set; } = 5;
    }

    public class TokenPolicyOptions
    {
        public int TokenLimit { get; set; } = 100;
        public int QueueLimit { get; set; } = 5;
        public int ReplenishmentPeriodSeconds { get; set; } = 10;
        public int TokensPerPeriod { get; set; } = 20;
        public bool AutoReplenishment { get; set; } = true;
    }

    public class ConcurrencyPolicyOptions
    {
        public int PermitLimit { get; set; } = 10;
        public int QueueLimit { get; set; } = 5;
    }

    public class PerIPPolicyOptions
    {
        public int PermitLimit { get; set; } = 100;
        public int WindowMinutes { get; set; } = 1;
        public bool AutoReplenishment { get; set; } = true;
    }

    public class PerUserPolicyOptions
    {
        public int PermitLimit { get; set; } = 50;
        public int WindowMinutes { get; set; } = 1;
        public int SegmentsPerWindow { get; set; } = 4;
        public bool AutoReplenishment { get; set; } = true;
    }

    public class TieredPolicyOptions
    {
        public TokenBucketOptions Premium { get; set; } = new();
        public TokenBucketOptions Basic { get; set; } = new();
        public TokenBucketOptions Anonymous { get; set; } = new();
    }

    public class TokenBucketOptions
    {
        public int TokenLimit { get; set; }
        public int ReplenishmentPeriodMinutes { get; set; }
        public int TokensPerPeriod { get; set; }
    }

    public class GlobalLimiterOptions
    {
        public int PermitLimit { get; set; } = 5;
        public int WindowMinutes { get; set; } = 1;
        public bool AutoReplenishment { get; set; } = true;
    }

    public class OnRejectedOptions
    {
        public int StatusCode { get; set; } = 429;
        public string Message { get; set; } = "Too many requests. Please try again later.";
        public string RetryAfterMessage { get; set; } = "Too many requests. Please try again after {0} minute(s).";
    }
}