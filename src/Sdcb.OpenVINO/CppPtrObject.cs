using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a base class for managing native resources.
/// Implements the <see cref="System.IDisposable"/> interface.
/// </summary>
public abstract class CppPtrObject : CppObject
{
    /// <summary>
    /// The handle to the native resource.
    /// </summary>
    protected IntPtr Handle;

    /// <inheritdoc/>
    public override bool Disposed => Handle == IntPtr.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="CppPtrObject"/> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public CppPtrObject(IntPtr handle, bool owned = true) : base(owned)
    {
        Handle = handle;
    }

    /// <summary>
    /// Releases the unmanaged resources.
    /// </summary>
    /// <param name="disposing">
    /// If true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
    /// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects.
    /// Only unmanaged resources can be disposed.
    /// </param>
    protected override void Dispose(bool disposing)
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