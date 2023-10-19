using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a base class for managing native resources.
/// Implements the <see cref="IDisposable"/> interface.
/// </summary>
public abstract class CppPtrObject : IDisposable
{
    /// <summary>
    /// The handle to the native resource.
    /// </summary>
    protected IntPtr Handle;

    /// <summary>
    /// Determines whether the instance owns the underlying C++ object.
    /// </summary>
    protected readonly bool Owned;

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    public virtual bool Disposed => Handle == IntPtr.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="CppPtrObject"/> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public CppPtrObject(IntPtr handle, bool owned = true)
    {
        Owned = owned;
        if (handle == IntPtr.Zero) throw new ArgumentNullException(nameof(handle));

        Handle = handle;
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
    /// Releases the native (unmanaged) resource.
    /// </summary>
    protected abstract void ReleaseCore();

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> exception if the object is disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    /// <summary>
    /// Destroys the object and releases its unmanaged resources when they are no longer needed.
    /// </summary>
    ~CppPtrObject()
    {
        Dispose(false);
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
            ReleaseCore();
        }
        Handle = IntPtr.Zero;
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
}