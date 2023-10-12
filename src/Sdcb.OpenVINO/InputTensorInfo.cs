using System;
using System.Runtime.InteropServices;
using System.Text;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents a wrapper class for preprocessing the input tensor in OpenVINO.
/// </summary>
public class InputTensorInfo : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputTensorInfo"/> class.
    /// </summary>
    /// <param name="handle">A handle to the underlying unmanaged object.</param>
    /// <param name="owned">Indicates whether this instance owns the handle.</param>
    public InputTensorInfo(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputTensorInfo"/> class.
    /// </summary>
    /// <param name="ptr">A pointer to the underlying unmanaged object.</param>
    /// <param name="owned">Indicates whether this instance owns the handle.</param>
    public unsafe InputTensorInfo(ov_preprocess_input_tensor_info* ptr, bool owned = true) : this((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// Sets the color format of the input tensor.
    /// </summary>
    public unsafe ov_color_format_e ColorFormat
    {
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_preprocess_input_tensor_info_set_color_format((ov_preprocess_input_tensor_info*)Handle, value));
        }
    }

    /// <summary>
    /// Sets the element type of the input tensor.
    /// </summary>
    public unsafe ov_element_type_e ElementType
    {
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_preprocess_input_tensor_info_set_element_type((ov_preprocess_input_tensor_info*)Handle, value));
        }
    }

    /// <summary>
    /// Sets the color format with subname(s) for the input tensor.
    /// </summary>
    /// <param name="color">The color format.</param>
    /// <param name="subNames">The subName(s) associated with the color format.</param>
    public unsafe void SetColorFormatWithSubName(ov_color_format_e color, params string[] subNames)
    {
        ThrowIfDisposed();

        GCHandle[] gchs = new GCHandle[subNames.Length];
        IntPtr[] p = new IntPtr[subNames.Length];
        for (int i = 0; i < subNames.Length; i++)
        {
            gchs[i] = GCHandle.Alloc(Encoding.UTF8.GetBytes(subNames[i] + '\0'), GCHandleType.Pinned);
            p[i] = gchs[i].AddrOfPinnedObject();
        }

        try
        {
            ov_preprocess_input_tensor_info* ptr = (ov_preprocess_input_tensor_info*)Handle;
            OpenVINOException.ThrowIfFailed(subNames.Length switch
            {
                0 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length),
                1 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0]),
                2 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1]),
                3 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2]),
                4 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4]),
                5 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4], p[5]),
                6 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4], p[5], p[6]),
                7 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4], p[5], p[6], p[7]),
                8 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4], p[5], p[6], p[7], p[8]),
                9 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4], p[5], p[6], p[7], p[8], p[9]),
                10 => ov_preprocess_input_tensor_info_set_color_format_with_subname(ptr, color, subNames.Length, p[0], p[1], p[2], p[4], p[5], p[6], p[7], p[8], p[9], p[10]),
                _ => throw new ArgumentOutOfRangeException(nameof(subNames), "The number of subNames must be less than or equal to 10."),
            });
        }
        finally
        {
            foreach (GCHandle handle in gchs)
            {
                handle.Free();
            }
        }
    }

    /// <summary>
    /// Sets the spatial static shape of the input tensor.
    /// </summary>
    public unsafe (int height, int width) SpatialStaticShape
    {
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_preprocess_input_tensor_info_set_spatial_static_shape((ov_preprocess_input_tensor_info*)Handle, value.height, value.width));
        }
    }

    /// <summary>
    /// Sets the memory type of the input tensor.
    /// </summary>
    public unsafe string MemoryType
    {
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            ThrowIfDisposed();
            fixed (byte* ptr = Encoding.UTF8.GetBytes(value + '\0'))
            {
                OpenVINOException.ThrowIfFailed(ov_preprocess_input_tensor_info_set_memory_type((ov_preprocess_input_tensor_info*)Handle, ptr));
            }
        }
    }

    /// <summary>Helper function to reuse element type and shape from user&apos;s created tensor.</summary>
    /// <param name="tensor">The tensor to retrive properties.</param>
    public unsafe void SetFrom(Tensor tensor)
    {
        if (tensor == null) throw new ArgumentNullException(nameof(tensor));

        ThrowIfDisposed();
        OpenVINOException.ThrowIfFailed(ov_preprocess_input_tensor_info_set_from((ov_preprocess_input_tensor_info*)Handle, (ov_tensor*)tensor.DangerousGetHandle()));
    }

    /// <summary>
    /// Sets the layout of the input tensor.
    /// </summary>
    public unsafe Layout Layout
    {
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_preprocess_input_tensor_info_set_layout((ov_preprocess_input_tensor_info*)Handle, (ov_layout*)value.DangerousGetHandle()));
        }
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_preprocess_input_tensor_info_free((ov_preprocess_input_tensor_info*)Handle);
    }

    /// <summary>
    /// Returns a weak reference to this instance.
    /// </summary>
    /// <returns> A new <see cref="InputTensorInfo"/> that represents a weak reference to this instance.</returns>
    public InputTensorInfo WeakRef()
    {
        ThrowIfDisposed();

        return new InputTensorInfo(Handle, owned: false);
    }
}
