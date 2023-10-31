using Sdcb.OpenVINO.Natives;
using System;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO;

/// <summary>
/// Used as a local memory cache for the OpenVINO Tensor class.
/// It can have several implementations, such as pinning memory from an array or cloning a Mat reference to prevent it from being released.
/// </summary>
internal abstract class TensorBuffer : IDisposable
{
    public TensorBuffer(ov_element_type_e elementType)
    {
        ElementType = elementType;
    }

    /// <summary>
    /// Gets or sets a boolean value indicating the state of the object.
    /// It indicates whether the object is disposed or not.
    /// </summary>
    public bool Disposed { get; protected set; }

    /// <summary>
    /// Gets the pointer to the data in the <see cref="TensorBuffer" />. 
    /// </summary>
    /// <exception cref="ObjectDisposedException" />
    public IntPtr DataPointer
    {
        get
        {
            ThrowIfDisposed();
            return GetDataPointer();
        }
    }

    /// <summary>Abstract method for fetching the data pointer.</summary>
    protected abstract IntPtr GetDataPointer();

    /// <summary>Fetching the data element type.</summary>
    public ov_element_type_e ElementType { get; }

    /// <summary>Checks whether the object has been disposed.</summary>
    /// <exception cref="ObjectDisposedException" />
    protected void ThrowIfDisposed()
    {
        if (Disposed) throw new ObjectDisposedException(GetType().Name);
    }

    /// <summary>
    /// Releases any resources used by the <see cref="TensorBuffer" /> and prevents the finalizer from running.
    /// It takes care of managed and unmanaged resources and then marks the object as disposed.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes resources, both managed and unmanaged, used by the <see cref="TensorBuffer" />.
    /// It relies on separate virtual methods for libraries to override: ReleaseManagedData and ReleaseUnmanagedData.
    /// It's called by Dispose() and the finalizer.
    /// </summary>
    /// <param name="disposing">A boolean indicating whether it is disposing managed resources or not </param>
    protected void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (disposing)
            {
                ReleaseManagedData();
            }
            ReleaseUnmanagedData();
            Disposed = true;
        }
    }

    /// <summary>
    /// Finalizer for the <see cref="TensorBuffer" />. Calls Dispose(false)
    /// </summary>
    ~TensorBuffer()
    {
        Dispose(false);
    }

    /// <summary>
    /// Virtual method for releasing managed data. 
    /// Nearly classes overriding this base class should provide their own release mechanism.
    /// </summary>
    protected virtual void ReleaseManagedData() { }

    /// <summary>
    /// Virtual method for releasing unmanaged data.
    /// Nearly classes overriding this base class should provide their own release mechanism.
    /// </summary>
    protected virtual void ReleaseUnmanagedData() { }
}

internal class ArrayTensorBuffer<T> : TensorBuffer where T : unmanaged
{
    private T[] _dataArray;
    private readonly GCHandle _handle;

    public unsafe ArrayTensorBuffer(T[] dataArraySrc) : base(GetElementType())
    {
        _dataArray = dataArraySrc;
        _handle = GCHandle.Alloc(dataArraySrc, GCHandleType.Pinned);
    }

    private static ov_element_type_e GetElementType()
    {
        Type t = typeof(T);
        return Type.GetTypeCode(t) switch
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
    }

    protected override IntPtr GetDataPointer()
    {
        return _handle.AddrOfPinnedObject();
    }

    protected override void ReleaseManagedData()
    {
        _dataArray = null!;
    }

    protected override void ReleaseUnmanagedData()
    {
        if (_handle.IsAllocated)
        {
            _handle.Free();
        }
    }

    public override string ToString()
    {
        return "<ArrayTensorBuffer " + "Handle IsAllocated: " + this._handle.IsAllocated + ">";
    }
}