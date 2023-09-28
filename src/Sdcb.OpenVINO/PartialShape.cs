using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a partial shape.
/// </summary>
public class PartialShape
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
    /// Overrides the <see cref="object.ToString"/> method to return the <see cref="PartialShape"/> as a string.
    /// </summary>
    /// <returns>A string representation of the <see cref="PartialShape"/>, including all dimensions.</returns>
    public override string ToString()
    {
        if (Rank.IsDynamic) return Rank.ToString();
        return $"{{{String.Join(",", Dimensions)}}}";
    }
}
