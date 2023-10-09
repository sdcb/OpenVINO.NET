using System;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents the NCHW (Number of Batches, Channels, Height, Width) format.
/// This is commonly used in convolutional neural networks.
/// </summary>
public readonly record struct NCHW(int NumberOfBatches, int Channels, int Height, int Width)
{
    /// <summary>
    /// Converts the NCHW object to a Shape object.
    /// </summary>
    /// <returns>
    /// A <see cref="Shape"/> object with dimensions corresponding to the properties of the current NCHW object.
    /// </returns>
    public readonly Shape ToShape() => new Shape(NumberOfBatches, Channels, Height, Width);

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

        return new NCHW((int)shape.Dimensions[0], (int)shape.Dimensions[1], (int)shape.Dimensions[2], (int)shape.Dimensions[3]);
    }

    /// <summary>
    /// Returns a string that represents the current NCHW object.
    /// </summary>
    /// <returns>
    /// A string that represents the current NCHW object.
    /// </returns>
    public readonly override string ToString() => $"NCHW({NumberOfBatches}, {Channels}, {Height}, {Width})";
}
