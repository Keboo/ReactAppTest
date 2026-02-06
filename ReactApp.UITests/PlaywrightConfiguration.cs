namespace ReactApp.UITests;

/// <summary>
/// Configuration settings for UI tests
/// </summary>
public static class PlaywrightConfiguration
{
    /// <summary>
    /// Timeout for page navigation and element visibility
    /// </summary>
    public static float DefaultTimeout => 30_000;
    
    /// <summary>
    /// Timeout for waiting on real-time SignalR updates
    /// </summary>
    public static float SignalRTimeout => 5_000;
    
    /// <summary>
    /// Whether to run browser in headless mode (no visible window)
    /// Defaults to true. Can be overridden via environment variable HEADLESS (set to "0" or "false" for headed mode)
    /// </summary>
    public static bool Headless
    {
        get
        {
            var envValue = Environment.GetEnvironmentVariable("HEADLESS");
            // Default to headless (true) unless explicitly set to false
            return envValue != "0" &&
                   !string.Equals(envValue, "false", StringComparison.OrdinalIgnoreCase);
        }
    }
    
    /// <summary>
    /// Slow motion delay in milliseconds (useful for debugging)
    /// Can be overridden via environment variable SLOW_MO
    /// </summary>
    public static float SlowMo
    {
        get
        {
            var envValue = Environment.GetEnvironmentVariable("SLOW_MO");
            return float.TryParse(envValue, out var value) ? value : 0;
        }
    }
}
