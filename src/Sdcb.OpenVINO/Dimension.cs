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
public readonly record struct Dimension(long Min, long Max)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Dimension"/> struct with the provided static value.
    /// </summary>
    /// <param name="staticValue">The static value to set both <see cref="Min"/> and <see cref="Max"/> to.</param>
    public Dimension(long staticValue) : this(staticValue, staticValue)
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
    public readonly bool IsDynamic => Min != Max || Max <= 0;

    /// <summary>
    /// Convert to <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator ov_dimension(Dimension dimension) => new() { min = dimension.Min, max = dimension.Max };

    /// <summary>
    /// Convert from <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator Dimension(ov_dimension rank) => new() { Min = rank.min, Max = rank.max };

    /// <summary>
    /// Convert from <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator Dimension(long rank) => new(rank);

    /// <summary>
    /// Gets a string representation of the <see cref="Dimension"/>.
    /// </summary>
    /// <returns>A string representation of the <see cref="Dimension"/>.</returns>
    public readonly override string ToString()
    {
        if (IsDynamic)
        {
            if (Min == -1 || Max == -1) return "?";
            return $"{Min}..{Max}";
        }
        else
        {
            return Min.ToString();
        }
    }
}

/// <summary>
/// Represents a rank with minimum and maximum values.
/// </summary>
/// <remarks>
/// The <see cref="Rank"/> class is a record struct that holds a minimum <see cref="Min"/> and maximum <see cref="Max"/> value.
/// </remarks>
/// <param name="Min">The minimum value of the rank.</param>
/// <param name="Max">The maximum value of the rank.</param>
public readonly record struct Rank(long Min, long Max)
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
    public readonly bool IsDynamic => Min != Max || Max <= 0;

    /// <summary>
    /// Convert to <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator ov_dimension(Rank rank) => new() { min = rank.Min, max = rank.Max };

    /// <summary>
    /// Convert from <see cref="ov_dimension"/>
    /// </summary>
    public static implicit operator Rank(ov_dimension rank) => new() { Min = rank.min, Max = rank.max };

    /// <summary>
    /// Gets a string representation of the <see cref="Rank"/>.
    /// </summary>
    /// <returns>A string representation of the <see cref="Rank"/>.</returns>
    public readonly override string ToString()
    {
        if (IsDynamic)
        {
            return "?";
        }
        else // IsStatic
        {
            return Min.ToString();
        }
    }
}
