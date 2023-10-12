using System;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents a pre and post-processing tool to be used before or after creating a model in OpenVINO
/// </summary>
public class PrePostProcessor : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrePostProcessor"/> class.
    /// </summary>
    /// <param name="ptr">The ov_preprocess_prepostprocessor pointer.</param>
    /// <param name="inputTensorCount">The input tensor count.</param>
    /// <param name="outputTensorCount">The output tensor count.</param>
    /// <param name="owned">if set to <c>true</c> [owned].</param>
    public unsafe PrePostProcessor(ov_preprocess_prepostprocessor* ptr, int inputTensorCount, int outputTensorCount, bool owned = true) : base((IntPtr)ptr, owned)
    {
        Inputs = new InputPrePostProcessorIndexer((ov_preprocess_prepostprocessor*)Handle, inputTensorCount);
        Outputs = new OutputPrePostProcessorIndexer((ov_preprocess_prepostprocessor*)Handle, outputTensorCount);
    }

    /// <summary>
    /// Indexer for accessing input pre-processing information.
    /// </summary>
    public PrePostProcessorIndexer<PreProcessInputInfo> Inputs { get; }

    /// <summary>
    /// Indexer for accessing output pre-processing information.
    /// </summary>
    public PrePostProcessorIndexer<PreProcessOutputInfo> Outputs { get; }

    /// <summary>
    /// Builds a model after input pre-processing.
    /// </summary>
    /// <returns>A model instance</returns>
    public unsafe Model BuildModel()
    {
        ThrowIfDisposed();

        ov_model* model;
        OpenVINOException.ThrowIfFailed(ov_preprocess_prepostprocessor_build((ov_preprocess_prepostprocessor*)Handle, &model));
        return new Model(model, true);
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_preprocess_prepostprocessor_free((ov_preprocess_prepostprocessor*)Handle);
    }
}
