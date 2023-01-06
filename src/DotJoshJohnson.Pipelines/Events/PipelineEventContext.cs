using DotJoshJohnson.Pipelines.Components;

namespace DotJoshJohnson.Pipelines.Events;

/// <summary>
/// Represents the current pipeline state.
/// </summary>
/// <typeparam name="TPipelineContext"></typeparam>
public class PipelineEventContext<TPipelineContext>
{
    internal PipelineEventContext(PipelineEventType eventType)
    {
        EventType = eventType;
    }

    /// <summary>
    /// The type of the component that has been or will be invoked (depending on the current point in the lifecycle).
    /// If a delegate is being invoked instead of a <see cref="IPipelineComponent{TContext}"/> implementation, this will be the type of the delegate.
    /// </summary>
    public Type? ComponentType { get; init; }

    /// <summary>
    /// The instance of the component that has been or will be invoked (depending on the current point in the lifecycle).
    /// If a delegate is being invoked instead of an <see cref="IPipelineComponent{TContext}"/> implementation, this will be null.
    /// </summary>
    public IPipelineComponent<TPipelineContext>? ComponentInstance { get; init; }

    /// <summary>
    /// The last thrown exception.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// The current point in the pipeline lifecycle.
    /// </summary>
    public PipelineEventType EventType { get; private set; }

    /// <summary>
    /// The pipeline context instance.
    /// </summary>
    public TPipelineContext PipelineContext { get; init; } = default!;

    internal PipelineEventContext<TPipelineContext> WithEventType(PipelineEventType eventType)
    {
        EventType = eventType;

        return this;
    }
}
