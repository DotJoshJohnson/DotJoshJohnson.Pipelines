namespace DotJoshJohnson.Pipelines;

public delegate Task PipelineInvocationDelegate<TContext>(TContext context, CancellationToken cancellationToken);