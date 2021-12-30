namespace DotJoshJohnson.Pipelines;

public delegate PipelineInvocationDelegate<TContext> PipelineConfigurationDelegate<TContext>(PipelineInvocationDelegate<TContext> next);
