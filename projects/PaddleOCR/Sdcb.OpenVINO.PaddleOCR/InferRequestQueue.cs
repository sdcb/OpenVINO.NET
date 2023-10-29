using System;
using System.Threading;

namespace Sdcb.OpenVINO.PaddleOCR;

public class InferRequestQueue : IDisposable
{
    private readonly CompiledModel _compiledModel;

    public InferRequestQueueOptions Options { get; }

    private readonly SemaphoreSlim? _semaphore;

    public InferRequestQueue(CompiledModel compiledModel, InferRequestQueueOptions options)
    {
        _compiledModel = compiledModel;
        Options = options;
        _semaphore = Options.ConsumerCount switch
        {
            > 0 => new SemaphoreSlim(Options.ConsumerCount),
            0 => new SemaphoreSlim(Math.Min(4, Environment.ProcessorCount)),
            < 0 => null,
        };
    }

    internal InferRequest Rent()
    {
        _semaphore?.Wait();
        return _compiledModel.CreateInferRequest();
    }

    internal void Returns()
    {
        _semaphore?.Release();
    }

    public InferRequestWrapper Using()
    {
        // rent and using
        return new InferRequestWrapper(this);
    }

    public void Dispose()
    {
        _compiledModel.Dispose();
        _semaphore?.Dispose();
    }
}

public static class CompiledModelExtensions
{
    public static InferRequestQueue CreateInferRequestQueue(this CompiledModel compiledModel, 
        InferRequestQueueOptions? options = null)
    {
        return new InferRequestQueue(compiledModel, options ?? InferRequestQueueOptions.Default);
    }
}

public class InferRequestWrapper : IDisposable
{
    private readonly InferRequestQueue _queue;
    public InferRequest InferRequest { get; }

    internal InferRequestWrapper(InferRequestQueue queue)
    {
        _queue = queue;
        InferRequest = queue.Rent();
    }

    public void Dispose()
    {
        if (!InferRequest.Disposed)
        {
            InferRequest.Dispose();
            _queue.Returns();
        }
    }
}

public readonly record struct InferRequestQueueOptions(int ConsumerCount)
{
    public InferRequestQueueType QueueType => ConsumerCount switch
    {
        < 0 => InferRequestQueueType.Unlimited,
        0 => InferRequestQueueType.Auto,
        > 0 => InferRequestQueueType.Fixed,
    };

    public static InferRequestQueueOptions Unlimited => new(-1);
    public static InferRequestQueueOptions Auto => new(0);
    public static InferRequestQueueOptions Default { get; set; } = Fixed(1);
    public static InferRequestQueueOptions Fixed(int consumerCount)
    {
        if (consumerCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(consumerCount), $"{nameof(consumerCount)} must greater than 0");
        }
        return new(consumerCount);
    }

    public override string ToString()
    {
        InferRequestQueueType queueType = QueueType;
        return queueType switch
        {
            InferRequestQueueType.Unlimited or InferRequestQueueType.Auto => $"{queueType}",
            InferRequestQueueType.Fixed => $"{queueType}: {ConsumerCount}",
            _ => throw new NotSupportedException($"{nameof(QueueType)}: {QueueType}.")
        };
    }
}

public enum InferRequestQueueType
{
    Unlimited = 0,
    Auto = 1,
    Fixed = 2,
}
