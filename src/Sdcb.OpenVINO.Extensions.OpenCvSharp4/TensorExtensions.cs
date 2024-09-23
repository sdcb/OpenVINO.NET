using OpenCvSharp;
using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO.Extensions.OpenCvSharp4;

/// <summary>
/// Provides extension methods for converting between <see cref="Mat"/> and <see cref="Tensor"/> objects.
/// </summary>
public static class TensorExtensions
{
    /// <summary>
    /// Creates a <see cref="Tensor"/> from a <see cref="Mat"/> object. 
    /// This method shares memory with the Mat, so if the Mat is disposed, the Tensor will also be invalidated.
    /// The shape of the Tensor is automatically determined from the Mat.
    /// </summary>
    /// <param name="mat">The input Mat object.</param>
    /// <returns>A Tensor that shares memory with the Mat.</returns>
    /// <exception cref="ArgumentNullException">Thrown when mat is null.</exception>
    /// <exception cref="NotSupportedException">Thrown when mat.MatType.Depth is not supported.</exception>
    public static unsafe Tensor AsTensor(this Mat mat)
    {
        if (mat == null) throw new ArgumentNullException(nameof(mat));

        return new Tensor(new MatTensorBuffer(mat), new Shape(1, mat.Height, mat.Width, mat.Type().Channels));
    }

    /// <summary>
    /// Creates a <see cref="Tensor"/> from a <see cref="Mat"/> object and stacks it as specified by the number of batches.
    /// This method shares memory with the Mat, so if the Mat is disposed, the Tensor will also be invalidated.
    /// The shape of the Tensor is automatically determined from the Mat and the number of batches.
    /// </summary>
    /// <param name="mat">The input Mat object.</param>
    /// <param name="numberOfBatches">The number of batches to stack the Tensor. Default value is 1.</param>
    /// <returns>A Tensor that shares memory with the Mat.</returns>
    /// <exception cref="ArgumentNullException">Thrown when mat is null.</exception>
    /// <exception cref="NotSupportedException">Thrown when mat.MatType.Depth is not supported.</exception>
    /// <exception cref="ArgumentException">Thrown when the height of the Mat is not divisible by the number of batches.</exception>
    public static unsafe Tensor StackedAsTensor(this Mat mat, int numberOfBatches = 1)
    {
        if (mat == null) throw new ArgumentNullException(nameof(mat));

        MatType matType = mat.Type();
        int channels = matType.Channels;
        Size size = mat.Size();
        int height = size.Height / numberOfBatches;
        if (height * numberOfBatches != size.Height)
        {
            throw new ArgumentException($"The height {size.Height} of the mat must be divisible by the number of batches {numberOfBatches}.");
        }

        return new Tensor(new MatTensorBuffer(mat), new NCHW(numberOfBatches, height, size.Width, channels));
    }

    /// <summary>
    /// Converts the <see cref="Tensor"/> to a <see cref="Mat"/> object. 
    /// This method shares memory with the Tensor.
    /// The width/height/depth of the generated Mat is automatically determined from the shape of the Tensor.
    /// It assumes that the Tensor's Layout is in NCHW format (if not, do not use this function).
    /// </summary>
    /// <returns>A Mat that shares memory with the Tensor and the shape is determined from the Tensor's Shape.</returns>
    /// <exception cref="NotSupportedException">Thrown when ElementType is not supported.</exception>
    public static Mat AsMat(this Tensor tensor)
    {
        NCHW nchw = tensor.Shape.ToNCHW();
        int depth = tensor.ElementType switch
        {
            ov_element_type_e.U8 => MatType.CV_8U,
            ov_element_type_e.I8 => MatType.CV_8S,
            ov_element_type_e.U16 => MatType.CV_16U,
            ov_element_type_e.I16 => MatType.CV_16S,
            ov_element_type_e.I32 => MatType.CV_32S,
            ov_element_type_e.F32 => MatType.CV_32F,
            ov_element_type_e.F64 => MatType.CV_64F,
            _ => throw new NotSupportedException($"ElementType ({tensor.ElementType}) is not supported.")
        };
        MatType matType = MatType.MakeType(depth, nchw.Channels);
        return Mat.FromPixelData(nchw.Height, nchw.Width, matType, tensor.DangerousGetHandle());
    }
}
