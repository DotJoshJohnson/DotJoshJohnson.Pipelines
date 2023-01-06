namespace DotJoshJohnson.Pipelines.Events;

public delegate void SimplePipelineEventHandlerDelegate<TContext>(PipelineEventContext<TContext> context);
