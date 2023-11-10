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
    private TensorBuffer? _tensorBuffer = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tensor"/> class.
    /// </summary>
    /// <param name="ptr">The handle to the tensor.</param>
    /// <param name="owned">Whether the handle is owned by this instance.</param>
    public unsafe Tensor(ov_tensor* ptr, bool owned = true) : base((IntPtr)ptr, owned)
    {
    }

    internal unsafe Tensor(TensorBuffer tensorBuffer, Shape shape, bool owned = true) : base((IntPtr)CreateFromBuffer(tensorBuffer, shape), owned)
    {
        _tensorBuffer = tensorBuffer;
    }

    private static unsafe ov_tensor* CreateFromBuffer(TensorBuffer tensorBuffer, Shape shape)
    {
        ov_tensor* ptr;
        using NativeShapeWrapper l = shape.Lock();
        OpenVINOException.ThrowIfFailed(ov_tensor_create_from_host_ptr(tensorBuffer.ElementType, l.Shape, (void*)tensorBuffer.DataPointer, & ptr));
        return ptr;
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

        return new Tensor(new ArrayTensorBuffer<T>(array), shape, owned: true);
    }

    /// <summary>
    /// Creates a new <see cref="Tensor"/> from an input array and given shape.
    /// </summary>
    /// <param name="array">The input array.</param>
    /// <returns>A new <see cref="Tensor"/> object with data copied from the input array.</returns>
    /// <exception cref="ArgumentException">Thrown if the input array does not have enough elements to fill the desired shape.</exception>
    /// <exception cref="NotSupportedException">Thrown if the type of the input data cannot be converted to a <see cref="Tensor"/> object.</exception>
    public static unsafe Tensor FromByteArray(byte[] array)
    {
        return new Tensor(new ArrayTensorBuffer<byte>(array), new Shape(1, array.Length), owned: true);
    }

    /// <summary>
    /// Creates a tensor from the provided data.
    /// This function shares the memory of the input data. 
    /// Callers should ensure that the data remains valid until the infer request has been completed.
    /// Failure to maintain valid data can lead to unpredictable behavior, including program crashes.
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
            ov_element_type_e.OV_BOOLEAN => 1,
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
    /// Caution should be exercised when using this method. The resulting Tensor will share the memory of the IntPtr data.
    /// It is the responsibility of the caller to ensure that this memory remains valid until the infer request has been completed.
    /// If the memory used by the IntPtr data becomes invalid, the Tensor will also become invalid, which may lead to unpredictable behavior, including program crashes.
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
        if (_tensorBuffer != null)
        {
            _tensorBuffer.Dispose();
            _tensorBuffer = null;
        }
    }
}
