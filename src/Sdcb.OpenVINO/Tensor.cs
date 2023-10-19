using OpenCvSharp;
using Sdcb.OpenVINO.Natives;
using System;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents a tensor in OpenVINO.
/// </summary>
public class Tensor : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tensor"/> class.
    /// </summary>
    /// <param name="ptr">The handle to the tensor.</param>
    /// <param name="owned">Whether the handle is owned by this instance.</param>
    public unsafe Tensor(ov_tensor* ptr, bool owned = true) : base((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tensor"/> class with the specified element type and shape.
    /// </summary>
    /// <param name="type">The element type of the tensor.</param>
    /// <param name="shape">The shape of the tensor.</param>
    public unsafe Tensor(ov_element_type_e type, Shape shape) : base((IntPtr)Create(type, shape), owned: true)
    {
    }

    private unsafe static ov_tensor* Create(ov_element_type_e type, Shape shape)
    {
        ov_tensor* ptr;
        using NativeShapeWrapper l = shape.Lock();
        OpenVINOException.ThrowIfFailed(ov_tensor_create(type, l.Shape, &ptr));
        return ptr;
    }

    /// <summary>
    /// Creates a <see cref="Tensor"/> from a <see cref="Mat"/> object. 
    /// This method shares memory with the Mat, so if the Mat is disposed, the Tensor will also be invalidated.
    /// The shape of the Tensor is automatically determined from the Mat.
    /// </summary>
    /// <param name="mat">The input Mat object.</param>
    /// <returns>A Tensor that shares memory with the Mat.</returns>
    /// <exception cref="ArgumentNullException">Thrown when mat is null.</exception>
    /// <exception cref="NotSupportedException">Thrown when mat.MatType.Depth is not supported.</exception>
    public static unsafe Tensor FromMat(Mat mat)
    {
        if (mat == null) throw new ArgumentNullException(nameof(mat));

        MatType matType = mat.Type();
        int channels = matType.Channels;
        ov_element_type_e elementType = matType.Depth switch
        {
            MatType.CV_8U => ov_element_type_e.U8, 
            MatType.CV_8S => ov_element_type_e.I8,
            MatType.CV_16U => ov_element_type_e.U16,
            MatType.CV_16S => ov_element_type_e.I16,
            MatType.CV_32S => ov_element_type_e.I32,
            MatType.CV_32F => ov_element_type_e.F32,
            MatType.CV_64F => ov_element_type_e.F64, 
            _ => throw new NotSupportedException($"Mat.MatType.Depth ({matType.Depth}) is not supported.")
        };

        Size size = mat.Size();
        return FromRaw(new ReadOnlySpan<byte>(mat.DataPointer, (int)((long)mat.DataEnd - (long)mat.DataStart)), new NCHW(1, size.Height, size.Width, channels), elementType);
    }

    /// <summary>
    /// Converts the <see cref="Tensor"/> to a <see cref="Mat"/> object. 
    /// This method shares memory with the Tensor.
    /// The width/height/depth of the generated Mat is automatically determined from the shape of the Tensor.
    /// It assumes that the Tensor's Layout is in NCHW format (if not, do not use this function).
    /// </summary>
    /// <returns>A Mat that shares memory with the Tensor and the shape is determined from the Tensor's Shape.</returns>
    /// <exception cref="NotSupportedException">Thrown when ElementType is not supported.</exception>
    public Mat ToMat()
    {
        NCHW nchw = Shape.ToNCHW();
        int depth = ElementType switch
        {
            ov_element_type_e.U8 => MatType.CV_8U,
            ov_element_type_e.I8 => MatType.CV_8S,
            ov_element_type_e.U16 => MatType.CV_16U,
            ov_element_type_e.I16 => MatType.CV_16S,
            ov_element_type_e.I32 => MatType.CV_32S,
            ov_element_type_e.F32 => MatType.CV_32F,
            ov_element_type_e.F64 => MatType.CV_64F,
            _ => throw new NotSupportedException($"ElementType ({ElementType}) is not supported.")
        };
        MatType matType = MatType.MakeType(depth, nchw.Channels);
        return new Mat(nchw.Height, nchw.Width, matType, DangerousGetHandle());
    }

    /// <summary>
    /// Creates a new <see cref="Tensor"/> from an input array and given shape.
    /// </summary>
    /// <param name="array">The input array.</param>
    /// <param name="shape">The <see cref="OpenVINO.Shape"/> of the <see cref="Tensor"/>.</param>
    /// <returns>A new <see cref="Tensor"/> object with data copied from the input array.</returns>
    /// <exception cref="ArgumentException">Thrown if the input array does not have enough elements to fill the desired shape.</exception>
    /// <exception cref="NotSupportedException">Thrown if the type of the input data cannot be converted to a <see cref="Tensor"/> object.</exception>

    public static unsafe Tensor FromArray<T>(T[] array, Shape shape) where T : unmanaged
    {
        if (array.Length < shape.ElementCount)
        {
            throw new ArgumentException($"The input array must have at least {shape.ElementCount} elements, but only {array.Length} elements were found.");
        }

        Type t = typeof(T);
        TypeCode code = Type.GetTypeCode(t);
        ov_element_type_e type = code switch
        {
            TypeCode.Byte => ov_element_type_e.U8,
            TypeCode.SByte => ov_element_type_e.I8,

            TypeCode.Int16 => ov_element_type_e.I16,
            TypeCode.UInt16 => ov_element_type_e.U16,

            TypeCode.Int32 => ov_element_type_e.I32,
            TypeCode.UInt32 => ov_element_type_e.U32,

            TypeCode.Int64 => ov_element_type_e.I64,
            TypeCode.UInt64 => ov_element_type_e.U64,

            TypeCode.Single => ov_element_type_e.F32,
            TypeCode.Double => ov_element_type_e.F64,
#if NET6_0_OR_GREATER
            var _ when t == typeof(Half) => ov_element_type_e.F16,
#endif
            _ => throw new NotSupportedException($"Type {t.Name} is not supported when convert to {nameof(Tensor)}.")
        };

        GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
        ov_tensor* tensor;
        try
        {
            using NativeShapeWrapper l = shape.Lock();
            OpenVINOException.ThrowIfFailed(ov_tensor_create_from_host_ptr(type, l.Shape, (void*)handle.AddrOfPinnedObject(), &tensor));
        }
        finally
        {
            handle.Free();
        }

        return new Tensor(tensor);
    }

    /// <summary>
    /// Creates a tensor from the provided data.
    /// </summary>
    /// <param name="data">A read-only span of bytes that represents the data for the tensor.</param>
    /// <param name="shape">A shape representing the dimensions of the tensor.</param>
    /// <param name="rawType">An element type that specifies the type of data that the tensor will hold. Default is U8.</param>
    /// <returns>A new Tensor containing the provided data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported type is provided.</exception>
    /// <exception cref="ArgumentException">Thrown when the size of the input array is less than the required minimum size for the shape.</exception>
    public static unsafe Tensor FromRaw(ReadOnlySpan<byte> data, Shape shape, ov_element_type_e rawType = ov_element_type_e.U8)
    {
        double unitSize = rawType switch
        {
            ov_element_type_e.U8 => 1,
            ov_element_type_e.F32 => 4,
            ov_element_type_e.BOOLEAN => 1,
            ov_element_type_e.BF16 => 2,
            ov_element_type_e.F16 => 2,
            ov_element_type_e.F64 => 8,
            ov_element_type_e.I4 => 0.5, // 4 bits, 0.5 bytes
            ov_element_type_e.I8 => 1,
            ov_element_type_e.I16 => 2,
            ov_element_type_e.I32 => 4,
            ov_element_type_e.I64 => 8,
            ov_element_type_e.U1 => 0.125, // 1 bit, 0.125 bytes
            ov_element_type_e.U4 => 0.5, // 4 bits, 0.5 bytes
            ov_element_type_e.U16 => 2,
            ov_element_type_e.U32 => 4,
            ov_element_type_e.U64 => 8,
            _ => throw new InvalidOperationException($"Unsupported type: {rawType}")
        };

        if (data.Length / unitSize < shape.ElementCount)
        {
            throw new ArgumentException($"The input array must have at least {shape.ElementCount} elements, but only {data.Length / unitSize} elements were found.");
        }

        fixed (byte* dataPtr = data)
        {
            return FromRaw((IntPtr)dataPtr, shape, rawType);
        }
    }

    /// <summary>
    /// Create a Tensor from raw data.
    /// </summary>
    /// <remarks>
    /// Be careful when using this method. The resulting Tensor will share the memory used by the IntPtr data.
    /// If the memory used by IntPtr data becomes invalid, the Tensor will also become invalid.
    /// </remarks>
    /// <param name="data">The IntPtr to the raw data.</param>
    /// <param name="shape">The shape of the tensor.</param>
    /// <param name="rawType">The type of data. Default is ov_element_type_e.U8.</param>
    /// <returns>A Tensor that uses the raw data.</returns>
    public static unsafe Tensor FromRaw(IntPtr data, Shape shape, ov_element_type_e rawType = ov_element_type_e.U8)
    {
        ov_tensor* tensor;
        using NativeShapeWrapper l = shape.Lock();
        OpenVINOException.ThrowIfFailed(ov_tensor_create_from_host_ptr(rawType, l.Shape, (void*)data, &tensor));
        return new Tensor(tensor, owned: true);
    }

    /// <summary>
    /// Gets or sets the shape of the tensor.
    /// </summary>
    public unsafe Shape Shape
    {
        get
        {
            ThrowIfDisposed();

            ov_shape_t nshape;
            try
            {
                OpenVINOException.ThrowIfFailed(ov_tensor_get_shape((ov_tensor*)Handle, &nshape));
                return new Shape(nshape);
            }
            finally
            {
                ov_shape_free(&nshape);
            }
        }
        set
        {
            ThrowIfDisposed();
            using NativeShapeWrapper locked = value.Lock();
            OpenVINOException.ThrowIfFailed(ov_tensor_set_shape((ov_tensor*)Handle, locked.Shape));
        }
    }

    /// <summary>
    /// Gets the size of the tensor in bytes.
    /// </summary>
    public unsafe long ByteSize
    {
        get
        {
            ThrowIfDisposed();

            nint byteSize;
            OpenVINOException.ThrowIfFailed(ov_tensor_get_byte_size((ov_tensor*)Handle, &byteSize));

            return byteSize;
        }
    }

    /// <summary>
    /// Gets the size of the tensor in elements.
    /// </summary>
    public unsafe long Size
    {
        get
        {
            ThrowIfDisposed();

            nint size;
            OpenVINOException.ThrowIfFailed(ov_tensor_get_size((ov_tensor*)Handle, &size));

            return size;
        }
    }

    /// <summary>
    /// Gets the element type of the tensor.
    /// </summary>
    public unsafe ov_element_type_e ElementType
    {
        get
        {
            ThrowIfDisposed();

            ov_element_type_e type;
            OpenVINOException.ThrowIfFailed(ov_tensor_get_element_type((ov_tensor*)Handle, &type));

            return type;
        }
    }

    /// <summary>
    /// This method retrieves the data pointer from an OpenVINO tensor. 
    /// It initiates the process in an unsafe context, which allows for direct memory manipulation.
    /// </summary>
    /// <remarks>
    /// This method is considered dangerous because it exposes a raw pointer, potentially leading to memory leaks or corruption if not handled correctly. 
    /// The method will throw an exception if the object it's being called on has been disposed.
    /// </remarks>
    /// <returns>
    /// The method returns an IntPtr that points to the tensor data. 
    /// </returns>
    /// <exception cref="System.ObjectDisposedException">Thrown when the method is invoked on a disposed object.</exception>
    /// <exception cref="OpenVINOException">Thrown when the tensor data retrieval fails.</exception>
    public unsafe IntPtr DangerousGetDataPtr()
    {
        ThrowIfDisposed();

        void* data;
        OpenVINOException.ThrowIfFailed(ov_tensor_data((ov_tensor*)Handle, &data));

        return (IntPtr)data;
    }

    /// <summary>
    /// Retrieves the tensor data and presents it as a <see cref="Span{T}"/> of unmanaged type T.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items in the <see cref="Span{T}"/>. T must be unmanaged.
    /// </typeparam>
    /// <remarks>
    /// This method is considered potentially dangerous because it involves direct memory manipulation via the <see cref="DangerousGetDataPtr"/> method.
    /// It wraps the retrieved pointer in a <see cref="Span{T}"/> providing a safe and bounds-checked mechanism for accessing the memory.
    /// </remarks>
    /// <returns>
    /// A <see cref="Span{T}"/> representing the tensor data in memory.
    /// </returns>
    /// <exception cref="System.OverflowException">Thrown if ByteSize/sizeof(T) results in a value too large for an int.</exception>
    /// <exception cref="System.ObjectDisposedException">Thrown when the method is invoked on a disposed object.</exception>
    /// <exception cref="OpenVINOException">Thrown when the tensor data retrieval from <see cref="DangerousGetDataPtr"/> fails.</exception>
    public unsafe Span<T> GetData<T>() where T : unmanaged
    {
        return new Span<T>((void*)DangerousGetDataPtr(), (int)(ByteSize / sizeof(T)));
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_tensor_free((ov_tensor*)Handle);
    }
}
