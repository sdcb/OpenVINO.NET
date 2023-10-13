using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// The PrePostProcessIndexer is an abstract class that implements the IReadOnlyList interface. 
/// It is a container class that stores <see cref="PreProcessInputInfo"/> or <see cref="PreProcessOutputInfo"/> objects. 
/// This class is used to enable users to perform pre-processing or post-processing for the input/output blobs of the primary inference network. 
/// </summary>
/// <typeparam name="T">The type of element in the indexer.</typeparam>
public abstract class PrePostProcessorIndexer<T> : IReadOnlyList<T>
{
    internal readonly unsafe ov_preprocess_prepostprocessor* _ptr;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrePostProcessorIndexer{T}"/> class.
    /// </summary>
    /// <param name="ptr">A pointer to the native <c>ov_preprocess_prepostprocessor</c> struct.</param>
    /// <param name="count">The number of elements in the indexer.</param>
    internal unsafe PrePostProcessorIndexer(ov_preprocess_prepostprocessor* ptr, int count)
    {
        _ptr = ptr;
        Count = count;
    }

    /// <summary>
    /// Gets the element with the specified name.
    /// </summary>
    /// <param name="name">The name of the element to get.</param>
    /// <returns>The element with the specified name.</returns>
    public abstract T this[string name] { get; }

    /// <inheritdoc/>
    public abstract T this[int index] { get; }

    /// <summary>
    /// Gets the primary element of the indexer.
    /// </summary>
    public abstract T Primary { get; }

    /// <inheritdoc/>
    public int Count { get; }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal unsafe void ThrowIfDisposed()
    {
        if (_ptr == null) throw new ObjectDisposedException($"{nameof(_ptr)} is null.");
    }
}

internal class InputPrePostProcessorIndexer : PrePostProcessorIndexer<PreProcessInputInfo>
{
    public unsafe InputPrePostProcessorIndexer(ov_preprocess_prepostprocessor* ptr, int count) : base(ptr, count)
    {
    }

    public unsafe override PreProcessInputInfo this[string name]
    {
        get
        {
            ov_preprocess_input_info* inputInfo;
            fixed (byte* namePtr = Encoding.UTF8.GetBytes(name + '\0'))
            {
                OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_get_input_info_by_name(_ptr, namePtr, &inputInfo));
            }

            return new PreProcessInputInfo(inputInfo, owned: true);
        }
    }

    public unsafe override PreProcessInputInfo this[int index]
    {
        get
        {
            ov_preprocess_input_info* inputInfo;
            OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_get_input_info_by_index(_ptr, index, &inputInfo));
            return new PreProcessInputInfo(inputInfo, owned: true);
        }
    }

    public unsafe override PreProcessInputInfo Primary
    {
        get
        {
            ov_preprocess_input_info* inputInfo;
            OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_get_input_info(_ptr, &inputInfo));
            return new PreProcessInputInfo(inputInfo, owned: true);
        }
    }
}

internal class OutputPrePostProcessorIndexer : PrePostProcessorIndexer<PreProcessOutputInfo>
{
    public unsafe OutputPrePostProcessorIndexer(ov_preprocess_prepostprocessor* ptr, int count) : base(ptr, count)
    {
    }

    public unsafe override PreProcessOutputInfo this[string name]
    {
        get
        {
            ov_preprocess_output_info* inputInfo;
            fixed (byte* namePtr = Encoding.UTF8.GetBytes(name + '\0'))
            {
                OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_get_output_info_by_name(_ptr, namePtr, &inputInfo));
            }

            return new PreProcessOutputInfo(inputInfo, owned: true);
        }
    }

    public unsafe override PreProcessOutputInfo this[int index]
    {
        get
        {
            ov_preprocess_output_info* inputInfo;
            OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_get_output_info_by_index(_ptr, index, &inputInfo));
            return new PreProcessOutputInfo(inputInfo, owned: true);
        }
    }

    public unsafe override PreProcessOutputInfo Primary
    {
        get
        {
            ov_preprocess_output_info* inputInfo;
            OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_get_output_info(_ptr, &inputInfo));
            return new PreProcessOutputInfo(inputInfo, owned: true);
        }
    }
}