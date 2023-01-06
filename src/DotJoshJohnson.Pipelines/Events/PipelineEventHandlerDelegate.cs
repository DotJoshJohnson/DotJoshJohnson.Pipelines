namespace DotJoshJohnson.Pipelines.Events;

public delegate Task PipelineEventHandlerDelegate<TContext>(PipelineEventContext<TContext> context, CancellationToken cancellationToken);
