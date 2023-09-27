using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a base class for the OpenVINO classes wrapping corresponding C++ objects.
/// </summary>
public abstract class CppObject : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CppObject"/> class.
    /// </summary>
    /// <param name="owned">If set to <c>true</c>, the instance owns the underlying C++ object.</param>
    public CppObject(bool owned)
    {
        Owned = owned;
    }

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
    public abstract bool Disposed { get; }

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
    /// Releases the unmanaged resources held by the object.
    /// </summary>
    /// <param name="disposing">
    /// If <c>true</c>, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
    /// If <c>false</c>, the method has been called by the runtime from inside the finalizer and you should not reference other objects.
    /// Only unmanaged resources can be disposed.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed && Owned)
        {
            ReleaseCore();
        }
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> exception if the object is disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (Disposed)
        {
            throw new ObjectDisposedException(nameof(CppObject));
        }
    }

    /// <summary>
    /// Destroys the object and releases its unmanaged resources when they are no longer needed.
    /// </summary>
    ~CppObject()
    {
        Dispose(false);
    }
}
