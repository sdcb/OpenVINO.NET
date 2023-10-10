using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents the NCHW (Number of Batches, Channels, Height, Width) format.
/// This is commonly used in convolutional neural networks.
/// </summary>
public readonly record struct NCHW(int NumberOfBatches, int Channels, int Height, int Width) : IEquatable<NCHW>
{
    /// <summary>
    /// Converts the NCHW object to a Shape object.
    /// </summary>
    /// <returns>
    /// A <see cref="Shape"/> object with dimensions corresponding to the properties of the current NCHW object.
    /// </returns>
    public readonly Shape ToShape() => new(NumberOfBatches, Channels, Height, Width);

    /// <summary>
    /// Converts a Shape object to an NCHW object.
    /// </summary>
    /// <param name="shape">
    /// The Shape object to be converted.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided Shape object does not have a rank of 4.
    /// </exception>
    /// <returns>
    /// A new <see cref="NCHW"/> object with properties set from the dimensions of the provided Shape object.
    /// </returns>
    public static NCHW FromShape(Shape shape)
    {
        if (shape.Rank != 4)
        {
            throw new ArgumentException("Shape must have a rank of 4 to be converted to NCHW.", nameof(shape));
        }

        return new NCHW(shape[0], shape[1], shape[2], shape[3]);
    }

    /// <summary>
    /// Implicitly converts an <see cref="NCHW"/> object to a <see cref="Shape"/> object.
    /// </summary>
    /// <param name="me">
    /// The <see cref="NCHW"/> object to be converted.
    /// </param>
    /// <returns>
    /// A <see cref="Shape"/> object with dimensions corresponding to the properties of the current NCHW object.
    /// </returns>
    public static implicit operator Shape(NCHW me) => me.ToShape();

    /// <summary>
    /// Returns a string that represents the current NCHW object.
    /// </summary>
    /// <returns>
    /// A string that represents the current NCHW object.
    /// </returns>
    public readonly override string ToString() => $"NCHW({NumberOfBatches}, {Channels}, {Height}, {Width})";
}
