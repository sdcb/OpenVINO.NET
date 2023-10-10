namespace Sdcb.OpenVINO;

/// <summary>
/// Enumeration representing performance modes for OpenVINO.
/// </summary>
public enum PerformanceMode
{
    /// <summary>
    /// Optimize for latency
    /// </summary>
    Latency = 1,

    /// <summary>
    /// Optimize for throughput
    /// </summary>
    Throughput = 2,

    /// <summary>
    /// Optimize for cumulative throughput
    /// </summary>
    CumulativeThroughput = 3,
}
