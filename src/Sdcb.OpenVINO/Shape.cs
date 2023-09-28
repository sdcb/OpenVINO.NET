using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a shape with managed memory and allows manipulation of each element.
/// </summary>
public class Shape : IEquatable<Shape>
{
    /// <summary>
    /// Gets or sets the dimensions of the shape.
    /// </summary>
    /// <remarks>
    /// The dimensions of the shape are represented as a one-dimensional array of 64-bit signed integers.
    /// </remarks>
    public long[] Dimensions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Shape"/> class with an OpenVINO shape <see cref="ov_shape_t"/>.
    /// </summary>
    /// <param name="shape">The OpenVINO <see cref="ov_shape_t"/> to initialize the <see cref="Shape"/> with.</param>
    public unsafe Shape(in ov_shape_t shape)
    {
        Dimensions = new long[shape.rank];
        for (int i = 0; i < shape.rank; ++i)
        {
            Dimensions[i] = shape.dims[i];
        }
    }

    /// <summary>
    /// Represents a shape with managed memory and allows manipulation of each element.
    /// </summary>
    /// <param name="shape">A list of integers representing the shape of the tensor.</param>
    public Shape(params long[] shape)
    {
        Dimensions = shape;
    }

    /// <summary>
    /// Compares this instance with an object of the same type and determines whether they are equal in values.
    /// </summary>
    /// <param name="other"><see cref="Shape"/> instance to compare with.</param>
    /// <returns><c>true</c> if instances are equal, <c>false</c> otherwise.</returns>
    public bool Equals(Shape? other)
    {
        if (other is not null)
            return Dimensions.SequenceEqual(other.Dimensions);
        return false;
    }

    /// <summary>
    /// Compares this instance with an object type and determines whether they are equal in values.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns><c>true</c> if the objects are equal, <c>false</c> otherwise.</returns>
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Shape shape => Equals(shape),
            _ => false,
        };
    }

    /// <summary>
    /// Checks whether two <see cref="Shape"/> instances are equal in values.
    /// </summary>
    /// <param name="a">First <see cref="Shape"/> instance.</param>
    /// <param name="b">Second <see cref="Shape"/> instance.</param>
    /// <returns><c>true</c> if instances are equal, <c>false</c> otherwise.</returns>
    public static bool operator ==(Shape a, Shape b) => a.Equals(b);

    /// <summary>
    /// Checks whether two <see cref="Shape"/> instances are not equal in values.
    /// </summary>
    /// <param name="a">First <see cref="Shape"/> instance.</param>
    /// <param name="b">Second <see cref="Shape"/> instance.</param>
    /// <returns><c>true</c> if instances are not equal, <c>false</c> otherwise.</returns>
    public static bool operator !=(Shape a, Shape b) => !a.Equals(b);

    /// <summary>
    /// Serves as a hash function for <see cref="Shape"/> instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        HashCode hashCode = new();

        foreach (long item in Dimensions)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Returns a string that represents <see cref="Shape"/> instance.
    /// </summary>
    /// <returns>A string that represents <see cref="Shape"/> instance.</returns>
    public override string ToString()
    {
        return $"{{{string.Join(",", Dimensions)}}}";
    }

    /// <summary>
    /// Locks the instance of the <see cref="Shape"/> class and returns an instance of <see cref="NativeShapeWrapper"/>.
    /// </summary>
    /// <returns>An instance of <see cref="NativeShapeWrapper"/>.</returns>
    public NativeShapeWrapper Lock() => new NativeShapeWrapper(this);
}
