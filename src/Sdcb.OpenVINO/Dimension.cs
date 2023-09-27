using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents a dimension with minimum and maximum values.
/// </summary>
/// <remarks>
/// The <see cref="Dimension"/> class is a record struct that holds a minimum <see cref="Min"/> and maximum <see cref="Max"/> value.
/// </remarks>
/// <param name="Min">The minimum value of the dimension.</param>
/// <param name="Max">The maximum value of the dimension.</param>
public record struct Dimension(long Min, long Max)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Dimension"/> struct with the provided static value.
    /// </summary>
    /// <param name="staticValue">The static value to set both <see cref="Min"/> and <see cref="Max"/> to.</param>
    public Dimension(int staticValue) : this(staticValue, staticValue)
    {
    }

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

/// <summary>
/// Represents a rank with minimum and maximum values.
/// </summary>
/// <remarks>
/// The <see cref="Rank"/> class is a record struct that holds a minimum <see cref="Min"/> and maximum <see cref="Max"/> value.
/// </remarks>
/// <param name="Min">The minimum value of the rank.</param>
/// <param name="Max">The maximum value of the rank.</param>
public record struct Rank(long Min, long Max)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rank"/> struct with the provided static value.
    /// </summary>
    /// <param name="staticValue">The static value to set both <see cref="Min"/> and <see cref="Max"/> to.</param>
    public Rank(int staticValue) : this(staticValue, staticValue)
    {
    }

    /// <summary>
    /// Gets a dynamic <see cref="Rank"/> with minimum and maximum values of -1.
    /// </summary>
    public static Rank Dynamic => new(-1, -1);

    /// <summary>
    /// Indicates whether the dimension is dynamic or not.
    /// </summary>
    /// <returns><c>true</c> if the dimension is dynamic, <c>false</c> otherwise.</returns>
    public readonly bool IsDynamic => ov_rank_is_dynamic(this);

    /// <summary>
    /// Convert to <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator ov_dimension(Rank rank) => new() { min = rank.Min, max = rank.Max };
}
