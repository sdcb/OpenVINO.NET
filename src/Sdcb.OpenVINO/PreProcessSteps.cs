using System;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// This class is for OpenVINO preprocessing steps.
/// </summary>
public class PreProcessSteps : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreProcessSteps"/> class with a handle.
    /// </summary>
    /// <param name="handle">The handle to be used.</param>
    /// <param name="owned">Indicates if the handle is owned or not. Defaults to true.</param>
    public PreProcessSteps(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreProcessSteps"/> class with a pointer.
    /// </summary>
    /// <param name="ptr">The pointer to the preprocess steps.</param>
    /// <param name="owned">Indicates if the pointer is owned or not. Defaults to true.</param>
    public unsafe PreProcessSteps(ov_preprocess_preprocess_steps* ptr, bool owned = true) : base((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// Adjusts the size of the preprocess steps with the specified algorithm.
    /// </summary>
    /// <param name="resizeAlgorithm">The resize algorithm to be used.</param>
    public unsafe void Resize(ov_preprocess_resize_algorithm_e resizeAlgorithm)
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_resize((ov_preprocess_preprocess_steps*)Handle, resizeAlgorithm));
    }

    /// <summary>
    /// Scales the preprocess steps by the specified divide value.
    /// </summary>
    /// <param name="scalingValue">The value to divide the preprocess steps with.</param>
    public unsafe void Scale(float scalingValue)
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_scale((ov_preprocess_preprocess_steps*)Handle, scalingValue));
    }

    /// <summary>Add scale preprocess operation. Divide each channel element of input by different specified value.</summary>
    /// <param name="scalingValues">Scaling values array for each channels</param>
    public unsafe void Scale(float[] scalingValues)
    {
        fixed (float* scaleChannelsPtr = scalingValues)
        {
            OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_scale_multi_channels((ov_preprocess_preprocess_steps*)Handle, scaleChannelsPtr, scalingValues.Length));
        }
    }

    /// <summary>
    /// Adjusts the mean of the preprocess steps by the specified subtraction value.
    /// </summary>
    /// <param name="subtractValue">The value to subtract from the mean.</param>
    public unsafe void Mean(float subtractValue)
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_mean((ov_preprocess_preprocess_steps*)Handle, subtractValue));
    }

    /// <summary>Add mean preprocess operation. Subtract each channel element of input by different specified value.</summary>
    /// <param name="subtractValues">Value array to subtract from each element.</param>
    public unsafe void Mean(float[] subtractValues)
    {
        fixed (float* meanChannelsPtr = subtractValues)
        {
            OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_mean_multi_channels((ov_preprocess_preprocess_steps*)Handle, meanChannelsPtr, subtractValues.Length));
        }
    }

    /// <summary>
    /// Crops the preprocess steps between the specified shape dimensions.
    /// </summary>
    /// <param name="begin">The starting shape for the crop.</param>
    /// <param name="end">The ending shape for the crop.</param>
    public unsafe void Crop(Shape begin, Shape end)
    {
        int[] beginArray = Array.ConvertAll(begin.Dimensions, x => (int)x);
        int[] endArray = Array.ConvertAll(end.Dimensions, x => (int)x);
        fixed (int* beginPtr = beginArray)
        fixed (int* endPtr = endArray)
        {
            OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_crop((ov_preprocess_preprocess_steps*)Handle,
                beginPtr, beginArray.Length,
                endPtr, endArray.Length));
        }
    }

    /// <summary>
    /// Converts the layout of the preprocess steps to the specified layout text.
    /// </summary>
    /// <param name="layoutText">The text of the layout to convert to.</param>
    public unsafe void ConvertLayout(string layoutText)
    {
        using Layout layout = new(layoutText);
        ConvertLayout(layout);
    }

    /// <summary>
    /// Converts the layout of the preprocess steps to the specified layout.
    /// </summary>
    /// <param name="layout">The layout to convert to.</param>
    public unsafe void ConvertLayout(Layout layout)
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_convert_layout((ov_preprocess_preprocess_steps*)Handle, (ov_layout*)layout.DangerousGetHandle()));
    }

    /// <summary>
    /// Reverses the channels of the preprocess steps.
    /// </summary>
    public unsafe void ReverseChannels()
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_reverse_channels((ov_preprocess_preprocess_steps*)Handle));
    }

    /// <summary>
    /// Converts the element type of the preprocess steps to the specified element type.
    /// </summary>
    /// <param name="elementType">The element type to convert to.</param>
    public unsafe void ConvertElementType(ov_element_type_e elementType)
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_convert_element_type((ov_preprocess_preprocess_steps*)Handle, elementType));
    }

    /// <summary>
    /// Converts the color of the preprocess steps to the specified color format.
    /// </summary>
    /// <param name="colorFormat">The color format to convert to.</param>
    public unsafe void ConvertColor(ov_color_format_e colorFormat)
    {
        OpenVINOException.ThrowIfFailed(ov_preprocess_preprocess_steps_convert_color((ov_preprocess_preprocess_steps*)Handle, colorFormat));
    }

    /// <summary>
    /// Returns a weak reference to this preprocess step.
    /// </summary>
    /// <returns>A weak reference to this preprocess step.</returns>
    public PreProcessSteps WeakRef()
    {
        ThrowIfDisposed();

        return new PreProcessSteps(Handle, false);
    }

    /// <summary>
    /// Releases the resources used by the <see cref="PreProcessSteps"/> object.
    /// </summary>
    protected unsafe override void ReleaseCore()
    {
        ov_preprocess_preprocess_steps_free((ov_preprocess_preprocess_steps*)Handle);
    }
}