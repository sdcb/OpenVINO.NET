using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents OpenVINO's InferRequest stream count definition. Setting this will have different effects on inference performance.
/// </summary>
public readonly record struct NumStreamsDef(int StreamCount)
{
    /// <summary>
    /// Represents the configuration where the number of streams is automatically set based on the device.
    /// </summary>
    public static NumStreamsDef Auto => new(-1);

    /// <summary>
    /// Represents the configuration optimized for Non-Uniform Memory Access (NUMA).
    /// </summary>
    public static NumStreamsDef Numa => new(-2);

    /// <summary>
    /// Converts the integer value to an instance of <see cref="NumStreamsDef" />.
    /// </summary>
    /// <param name="streamCount">The number of streams.</param>
    /// <returns>A new instance of <see cref="NumStreamsDef" /> with the given number of streams.</returns>
    public static implicit operator NumStreamsDef(int streamCount) => streamCount switch
    {
        < -2 => throw new ArgumentException("value must >= -2 or AUTO/NUMA", nameof(streamCount)),
        _ => new NumStreamsDef(streamCount),
    };

    /// <summary>
    /// Converts the <see cref="NumStreamsDef" /> instance to an integer.
    /// </summary>
    /// <param name="streamCount">The <see cref="NumStreamsDef" /> instance.</param>
    /// <returns>The number of streams.</returns>
    public static implicit operator int(NumStreamsDef streamCount) => streamCount;

    /// <summary>
    /// Parses a string and returns an object of <see cref="NumStreamsDef" />.
    /// </summary>
    /// <param name="streamCount">The number of streams as a string.</param>
    /// <returns>A new instance of <see cref="NumStreamsDef" /> created from the parsed string.</returns>
    public static NumStreamsDef Parse(string streamCount) => streamCount switch
    {
        "AUTO" => Auto,
        "NUMA" => Numa,
        _ => int.Parse(streamCount)
    };

    /// <summary>
    /// Returns a string representation of the stream count.
    /// </summary>
    /// <returns>A string that represents the current stream count.</returns>
    public override string ToString() => StreamCount switch
    {
        -1 => "AUTO",
        -2 => "NUMA",
        _ => StreamCount.ToString()
    };
}
