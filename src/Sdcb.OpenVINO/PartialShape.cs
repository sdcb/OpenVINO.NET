using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a partial shape.
/// </summary>
public readonly struct PartialShape : IEquatable<PartialShape>
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
    /// Gets or sets the dimension at the specified index.
    /// </summary>
    /// <param name="dimensionsIndex">The index of the dimension to get or set.</param>
    /// <returns>The dimension at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
    public Dimension this[int dimensionsIndex]
    {
        get => Dimensions[dimensionsIndex];
        set => Dimensions[dimensionsIndex] = value;
    }

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
    public unsafe PartialShape(params int[] dims)
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
    public bool Equals(PartialShape other)
    {
        return Rank == other.Rank && Dimensions.SequenceEqual(other.Dimensions);
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
    public override readonly string ToString()
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
        Dimension[] ds = new Dimension[shape.Rank];
        for (int i = 0; i < ds.Length; ++i)
        {
            ds[i] = new Dimension(shape[i]);
        }
        return new PartialShape(ds);
    }

    /// <summary>
    /// Determines if the current <see cref="PartialShape"/> instance is compatible with the specified <see cref="PartialShape"/>.
    /// </summary>
    /// <param name="s">
    /// The other <see cref="PartialShape"/> instance to compare with the current object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the current instance is compatible with <paramref name="s"/>.
    /// This method returns <c>true</c> under the following conditions:
    /// - Both instances' Ranks are dynamic.
    /// - Current instance's Rank is dynamic and <paramref name="s"/>'s is static.
    /// - Current instance's Rank is static and <paramref name="s"/>'s is dynamic.
    /// - Both instances' Ranks are static and equal, and if a Dimension is dynamic in any of the instances, 
    ///   or the Dimensions are equal in both instances for all corresponding indexes.
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// This method considers an instance is dynamic when <see cref="Rank"/> or any <see cref="Dimension"/> is dynamic.
    /// </remarks>
    public bool IsCompatible(PartialShape s)
    {
        // If we don't know *this's rank, or we don't know s's rank, they are compatible.
        if (Rank.IsDynamic || s.Rank.IsDynamic)
        {
            return true;
        }
        // If we do know *this's rank and s's rank, and they are unequal, they are incompatible.
        else if (Rank.Max != s.Rank.Max)
        {
            return false;
        }
        // If we know both the ranks and they are equal, then *this and s are compatible iff they
        // are elementwise compatible everywhere.
        else
        {
            for (int i = 0; i < Rank.Max; i++)
            {
                if (!Dimensions[i].IsDynamic && !s.Dimensions[i].IsDynamic && (Dimensions[i].Min != s.Dimensions[i].Min || Dimensions[i].Max != s.Dimensions[i].Max))
                {
                    return false;
                }
            }
            // If we are still here, we know that s1 and s2 have the same rank and are elementwise
            // compatible everywhere.
            return true;
        }
    }
}
