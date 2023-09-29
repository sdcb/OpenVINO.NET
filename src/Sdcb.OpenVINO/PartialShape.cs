using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a partial shape.
/// </summary>
public class PartialShape : IEquatable<PartialShape>
{
    /// <summary>
    /// The rank of the partial shape.
    /// </summary>
    public Rank Rank { get; }

    /// <summary>
    /// An array of dimensions that describe the partial shape.
    /// </summary>
    public Dimension[] Dimensions { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="PartialShape"/> object is dynamic.
    /// </summary>
    /// <remarks>
    /// A <see cref="PartialShape"/> object is dynamic if its <see cref="Rank"/> is dynamic or if any of its <see cref="Dimensions"/>
    /// have a dynamic value.
    /// </remarks>
    /// <returns>True if the <see cref="PartialShape"/> object is dynamic, false otherwise.</returns>
    public bool IsDynamic => Rank.IsDynamic || Dimensions.Any(x => x.IsDynamic);

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialShape"/> class with the specified openvino <see cref="ov_partial_shape"/>.
    /// </summary>
    /// <param name="shape">The openvino <see cref="ov_partial_shape"/> to initialize the new <see cref="PartialShape"/> object from.</param>
    /// <remarks>
    /// This constructor creates a new <see cref="PartialShape"/> object by copying the <paramref name="shape"/> array
    /// into a new array of <see cref="Dimension"/> objects.
    /// </remarks>
    public unsafe PartialShape(in ov_partial_shape shape)
    {
        Rank = shape.rank;
        if (Rank.IsDynamic)
        {
            Dimensions = Array.Empty<Dimension>();
        }
        else
        {
            Dimensions = new Dimension[Rank.Max];
            for (int i = 0; i < Dimensions.Length; i++)
            {
                Dimensions[i] = shape.dims[i];
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialShape"/> class that represents a dynamic rank.
    /// </summary>
    /// <param name="rank">The rank of the partial shape.</param>
    /// <param name="dimensions">An array of dimensions that describe the partial shape.</param>
    public unsafe PartialShape(Rank rank, params Dimension[] dimensions)
    {
        Rank = rank;
        if (rank.IsDynamic)
        {
            Dimensions = Array.Empty<Dimension>();
        }
        else
        {
            if (rank.Min != dimensions.Length)
            {
                throw new ArgumentException($"The number of dimensions provided {dimensions.Length} does not match the expected rank {rank.Min}.");
            }
            Dimensions = dimensions;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialShape"/> class that represents a dynamic dimension.
    /// </summary>
    /// <param name="dims">An array of dimensions that describe the partial shape.</param>
    public unsafe PartialShape(params Dimension[] dims)
    {
        Rank = new(dims.Length);
        Dimensions = dims;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialShape"/> class that represents a static dimension.
    /// </summary>
    /// <param name="dims">An array of dimensions that describe the partial shape.</param>
    public unsafe PartialShape(params long[] dims)
    {
        Rank = new(dims.Length);
        Dimensions = new Dimension[dims.Length];
        for (int i = 0; i < dims.Length; i++)
        {
            Dimensions[i] = new(dims[i]);
        }
    }

    /// <summary>
    /// Dynamic rank Partial Shape.
    /// </summary>
    public static PartialShape DynamicRank { get; } = new PartialShape(Rank.Dynamic);

    /// <summary>
    /// Indicates whether the current <see cref="PartialShape"/> object is equal to another <see cref="PartialShape"/> object.
    /// </summary>
    /// <param name="other">A <see cref="PartialShape"/> object to compare with this <see cref="PartialShape"/> object.</param>
    /// <returns>True if the two objects are equal, false otherwise.</returns>
    public bool Equals(PartialShape? other)
    {
        if (other is not null)
            return Rank == other.Rank && Dimensions.SequenceEqual(other.Dimensions);
        return false;
    }

    /// <summary>
    /// Compares this <see cref="PartialShape"/> with an object type and determines whether they are equal in values.
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
    /// Determines whether two specified <see cref="PartialShape"/> objects have the same value.
    /// </summary>
    /// <param name="left">The first <see cref="PartialShape"/> to compare.</param>
    /// <param name="right">The second <see cref="PartialShape"/> to compare.</param>
    /// <returns>True if the two objects have the same value, false otherwise.</returns>
    public static bool operator ==(PartialShape left, PartialShape right) => left.Equals(right);

    /// <summary>
    /// Determines whether two specified <see cref="PartialShape"/> objects have different values.
    /// </summary>
    /// <param name="left">The first <see cref="PartialShape"/> to compare.</param>
    /// <param name="right">The second <see cref="PartialShape"/> to compare.</param>
    /// <returns>True if the two objects have different values, false otherwise.</returns>
    public static bool operator !=(PartialShape left, PartialShape right) => !left.Equals(right);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <remarks>
    /// This method is used by data structures such as <see cref="Dictionary{TKey, TValue}"/> to determine the position
    /// at which an object should be stored or looked up.
    /// </remarks>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(Rank);
        foreach (Dimension dimension in Dimensions)
        {
            hashCode.Add(dimension.GetHashCode());
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Overrides the <see cref="object.ToString"/> method to return the <see cref="PartialShape"/> as a string.
    /// </summary>
    /// <returns>A string representation of the <see cref="PartialShape"/>, including all dimensions.</returns>
    public override string ToString()
    {
        if (Rank.IsDynamic) return Rank.ToString();
        return $"{{{string.Join(",", Dimensions)}}}";
    }

    /// <summary>
    /// Returns a new <see cref="NativePartialShapeWrapper"/> object for the current <see cref="PartialShape"/> object,
    /// used to pin the dimensions array in memory.
    /// </summary>
    /// <remarks>
    /// The <see cref="NativePartialShapeWrapper"/> object should be disposed of after use to free the memory allocated
    /// for the dimensions array.
    /// </remarks>
    /// <returns>A new <see cref="NativePartialShapeWrapper"/> object.</returns>
    public NativePartialShapeWrapper Lock() => new(this);

    /// <summary>
    /// Defines an explicit conversion of a <see cref="PartialShape"/> object to a <see cref="Shape"/> object.
    /// </summary>
    /// <param name="partialShape">The <see cref="PartialShape"/> object to convert to a <see cref="Shape"/> object.</param>
    /// <returns>The <see cref="Shape"/> object that was created from the <see cref="PartialShape"/> object.</returns>
    /// <exception cref="NotSupportedException">Thrown if the <paramref name="partialShape"/> is dynamic.</exception>
    public static explicit operator Shape(PartialShape partialShape)
    {
        if (partialShape.IsDynamic) throw new NotSupportedException($"Cannot convert dynamic {nameof(PartialShape)} to {nameof(Shape)}.");

        return new Shape(partialShape.Dimensions.Select(x => x.Max).ToArray());
    }

    /// <summary>
    /// Defines an implicit conversion of a <see cref="Shape"/> object to a <see cref="PartialShape"/> object.
    /// </summary>
    /// <param name="shape">The <see cref="Shape"/> object to convert to a <see cref="PartialShape"/> object.</param>
    /// <returns>The <see cref="PartialShape"/> object that was created from the <see cref="Shape"/> object.</returns>
    public static implicit operator PartialShape(Shape shape)
    {
        Dimension[] ds = new Dimension[shape.Dimensions.Length];
        for (int i = 0; i < ds.Length; ++i)
        {
            ds[i] = new Dimension(shape.Dimensions[i]);
        }
        return new PartialShape(ds);
    }
}
