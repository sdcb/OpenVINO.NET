using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a base class for managing native resources.
/// Implements the <see cref="System.IDisposable"/> interface.
/// </summary>
public abstract class NativeResource : IDisposable
{
    /// <summary>
    /// The handle to the native resource.
    /// </summary>
    protected IntPtr Handle;

    /// <summary>
    /// Determines whether the instance owns the handle.
    /// </summary>
    protected bool Owned;

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    public bool Disposed => Handle == IntPtr.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeResource"/> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public NativeResource(IntPtr handle, bool owned = true)
    {
        Handle = handle;
        Owned = owned;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resource.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources.
    /// </summary>
    /// <param name="disposing">
    /// If true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
    /// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects.
    /// Only unmanaged resources can be disposed.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed && Owned)
        {
            ReleaseHandle(Handle);
        }
        Handle = IntPtr.Zero;
    }

    /// <summary>
    /// Releases the native handle.
    /// </summary>
    /// <param name="handle">The native handle to release.</param>
    protected abstract void ReleaseHandle(IntPtr handle);

    /// <summary>
    /// Throws an exception if the handle has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException(nameof(Handle));
        }
    }

    /// <summary>
    /// Gets the native handle in a potentially dangerous way.
    /// </summary>
    /// <remarks>
    /// Callers should use this method with extreme caution, because it can return null or invalid handles. Only use this method when interacting with native API's that require a pointer to the native object.
    /// </remarks>
    /// <exception cref="System.ObjectDisposedException">Thrown when the object has been disposed and the handle is no longer valid.</exception>
    /// <returns>The native handle to the object.</returns>
    public IntPtr DangerousGetHandle()
    {
        ThrowIfDisposed();
        return Handle;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="NativeResource"/> class.
    /// </summary>
    ~NativeResource()
    {
        Dispose(false);
    }
}