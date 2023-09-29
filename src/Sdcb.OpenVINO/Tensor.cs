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
    /// <param name="handle">The handle to the tensor.</param>
    /// <param name="owned">Whether the handle is owned by this instance.</param>
    public Tensor(IntPtr handle, bool owned = true) : base(handle, owned)
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
    /// Creates a new <see cref="Tensor"/> from an input array and given shape.
    /// </summary>
    /// <param name="array">The input array.</param>
    /// <param name="shape">The <see cref="OpenVINO.Shape"/> of the <see cref="Tensor"/>.</param>
    /// <returns>A new <see cref="Tensor"/> object with data copied from the input array.</returns>
    /// <exception cref="ArgumentException">Thrown if the input array does not have enough elements to fill the desired shape.</exception>
    /// <exception cref="NotSupportedException">Thrown if the type of the input data cannot be converted to a <see cref="Tensor"/> object.</exception>

    public static unsafe Tensor From<T>(T[] array, Shape shape) where T : struct
    {
        if (array.Length < shape.ElementCount)
        {
            throw new ArgumentException($"The input array must have at least {shape.ElementCount} elements, but only {array.Length} elements were found.");
        }

        Type t = typeof(T);
        TypeCode code = Type.GetTypeCode(t);
        ov_element_type_e type = code switch
        { 
            TypeCode.Byte or TypeCode.SByte => ov_element_type_e.U8, 
            TypeCode.Int16 or TypeCode.UInt16 => ov_element_type_e.U16,
            TypeCode.Int32 or TypeCode.UInt32 => ov_element_type_e.U32,
            TypeCode.Int64 or TypeCode.UInt64 => ov_element_type_e.U64,
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

        return new Tensor((IntPtr)tensor);
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

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_tensor_free((ov_tensor*)Handle);
    }
}
