using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents a dimension with minimum and maximum values.
/// </summary>
/// <remarks>
/// The <see cref="Dimension"/> class is a record struct that holds a minimum <see cref="Min"/> and maximum <see cref="Max"/> value.
/// </remarks>
public record struct Dimension(long Min, long Max)
{
    /// <summary>
    /// Gets a dynamic <see cref="Dimension"/> with minimum and maximum values of -1.
    /// </summary>
    public static Dimension Dynamic => new(-1, -1);

    /// <summary>
    /// Indicates whether the dimension is dynamic or not.
    /// </summary>
    /// <returns><c>true</c> if the dimension is dynamic, <c>false</c> otherwise.</returns>
    public readonly bool IsDynamic => ov_dimension_is_dynamic(this);

    /// <summary>
    /// Convert to <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator ov_dimension(Dimension dimension) => new() { min = dimension.Min, max = dimension.Max };
}
