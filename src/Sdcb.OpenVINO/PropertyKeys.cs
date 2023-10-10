namespace Sdcb.OpenVINO;

/// <summary>
/// Known all OpenVINO property keys.
/// </summary>
public static class PropertyKeys
{
    #region Read-only property keys

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_supported_properties</c> in C++.
    /// </summary>
    public const string SupportedProperties = "SUPPORTED_PROPERTIES";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_available_devices</c> in C++.
    /// </summary>
    public const string AvailableDevices = "AVAILABLE_DEVICES";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_optimal_number_of_infer_requests</c> in C++.
    /// </summary>
    public const string OptimalNumberOfInferRequests = "OPTIMAL_NUMBER_OF_INFER_REQUESTS";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_range_for_async_infer_requests</c> in C++.
    /// </summary>
    public const string RangeForAsyncInferRequests = "RANGE_FOR_ASYNC_INFER_REQUESTS";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_range_for_streams</c> in C++.
    /// </summary>
    public const string RangeForStreams = "RANGE_FOR_STREAMS";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_device_full_name</c> in C++.
    /// </summary>
    public const string DeviceFullName = "FULL_DEVICE_NAME";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_device_capabilities</c> in C++.
    /// </summary>
    public const string DeviceCapabilities = "OPTIMIZATION_CAPABILITIES";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_model_name</c> in C++.
    /// </summary>
    public const string ModelName = "NETWORK_NAME";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_optimal_batch_size</c> in C++.
    /// </summary>
    public const string OptimalBatchSize = "OPTIMAL_BATCH_SIZE";

    /// <summary>
    /// (readonly) Corresponds to <c>ov_property_key_max_batch_size</c> in C++.
    /// </summary>
    public const string MaxBatchSize = "MAX_BATCH_SIZE";

    #endregion

    #region Read-write property keys

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_cache_dir</c> in C++.
    /// </summary>
    public const string CacheDir = "CACHE_DIR";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_num_streams</c> in C++.
    /// </summary>
    public const string NumStreams = "NUM_STREAMS";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_affinity</c> in C++.
    /// </summary>
    public const string Affinity = "AFFINITY";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_inference_num_threads</c> in C++.
    /// </summary>
    public const string InferenceNumThreads = "INFERENCE_NUM_THREADS";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_performance_mode</c> in C++.
    /// </summary>
    public const string HintPerformanceMode = "PERFORMANCE_HINT";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_enable_cpu_pinning</c> in C++.
    /// </summary>
    public const string HintEnableCpuPinning = "ENABLE_CPU_PINNING";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_scheduling_core_type</c> in C++.
    /// </summary>
    public const string HintSchedulingCoreType = "SCHEDULING_CORE_TYPE";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_enable_hyper_threading</c> in C++.
    /// </summary>
    public const string HintEnableHyperThreading = "ENABLE_HYPER_THREADING";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_inference_precision</c> in C++.
    /// </summary>
    public const string HintInferencePrecision = "INFERENCE_PRECISION_HINT";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_num_requests</c> in C++.
    /// </summary>
    public const string HintNumRequests = "PERFORMANCE_HINT_NUM_REQUESTS";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_model_priority</c> in C++.
    /// </summary>
    public const string HintModelPriority = "MODEL_PRIORITY";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_log_level</c> in C++.
    /// </summary>
    public const string LogLevel = "LOG_LEVEL";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_enable_profiling</c> in C++.
    /// </summary>
    public const string EnableProfiling = "PERF_COUNT";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_device_priorities</c> in C++.
    /// </summary>
    public const string DevicePriorities = "MULTI_DEVICE_PRIORITIES";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_hint_execution_mode</c> in C++.
    /// </summary>
    public const string HintExecutionMode = "EXECUTION_MODE_HINT";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_force_tbb_terminate</c> in C++.
    /// </summary>
    public const string ForceTbbTerminate = "FORCE_TBB_TERMINATE";

    /// <summary>
    /// (read-write) Corresponds to <c>ov_property_key_enable_mmap</c> in C++.
    /// </summary>
    public const string EnableMmap = "ENABLE_MMAP";

    #endregion
}