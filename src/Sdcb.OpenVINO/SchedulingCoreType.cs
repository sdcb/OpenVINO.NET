namespace Sdcb.OpenVINO;

/// <summary>
/// definition of core type can be used for CPU tasks on different devices.
/// </summary>
public enum SchedulingCoreType
{
    /// <summary>
    /// Any processors can be used.
    /// </summary>
    AnyCore = 0,

    /// <summary>
    /// Only processors of performance-cores can be used.
    /// </summary>
    PCoresOnly = 1,

    /// <summary>
    /// Only processors of efficient-cores can be used.
    /// </summary>
    ECoresOnly = 2,
}