using System;
using System.Text;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents the layout of OpenVINO in-memory data. 
/// </summary>
public class Layout : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Layout"/> class with the specified handle and owned status.
    /// </summary>
    /// <param name="handle">A pointer to the memory address of the layout data instance.</param>
    /// <param name="owned">Indicates whether the object owns the layout data instance.</param>
    public Layout(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Layout"/> class from a pointer to OpenVINO layout memory.
    /// </summary>
    /// <param name="ptr">A pointer to the memory address of the OpenVINO layout data instance.</param>
    /// <param name="owned">Indicates whether the object owns the layout data instance.</param>
    public unsafe Layout(ov_layout* ptr, bool owned = true) : base((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Layout"/> class from the specified layout text.
    /// </summary>
    /// <param name="layoutText">A string representation of the layout data instance.</param>
    public unsafe Layout(string layoutText) : this(FromString(layoutText), owned: true)
    {
    }

    private static Lazy<Layout> _NCHW = new (() => new Layout("NCHW"));

    /// <summary>
    /// NCHW represents the layout in memory for batches and channels data where the channel value changes the fastest, followed by the height, and then the width of the data.
    /// </summary>
    public static Layout NCHW => _NCHW.Value.WeakRef();

    private static Lazy<Layout> _NHWC = new(() => new Layout("NHWC"));

    /// <summary>
    /// NHWC represents the layout in memory for batches and channels data where the channel value changes last, followed by the height, and then the width of the data.
    /// </summary>
    public static Layout NHWC => _NHWC.Value.WeakRef();

    /// <summary>
    /// Returns a new instance of the <see cref="Layout"/> class that holds a weak reference to the underlying <see cref="CppPtrObject"/> instance.
    /// </summary>
    /// <returns>A new instance of the <see cref="Layout"/> class that holds a weak reference to the underlying <see cref="CppPtrObject"/> instance.</returns>
    public Layout WeakRef()
    {
        ThrowIfDisposed();
        return new Layout(Handle, owned: false);
    }

    private unsafe static ov_layout* FromString(string layoutText)
    {
        if (layoutText == null) throw new ArgumentNullException(nameof(layoutText));

        ov_layout* layout;
        fixed (byte* layoutPtr = Encoding.UTF8.GetBytes(layoutText + '\0'))
        {
            OpenVINOException.ThrowIfFailed(ov_layout_create(layoutPtr, &layout));
        }

        return layout;
    }

    /// <summary>
    /// Returns the string representation of the current instance.
    /// </summary>
    public unsafe override string ToString()
    {
        ThrowIfDisposed();

        byte* strPtr = ov_layout_to_string((ov_layout*)Handle);
        return StringUtils.UTF8PtrToString((IntPtr)strPtr)!;
    }

    /// <inheritdoc />
    protected unsafe override void ReleaseCore()
    {
        ov_layout_free((ov_layout*)Handle);
    }
}
