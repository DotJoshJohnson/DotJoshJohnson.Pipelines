namespace DotJoshJohnson.Pipelines.Components;

/// <summary>
/// A member of the pipeline that can do work and optionally call the next component in the pipeline or short-circuit the pipeline.
/// </summary>
public interface IPipelineComponent<TContext>
{
    /// <summary>
    /// Called by the pipeline when the pipeline starts (if this is the first component in the pipeline) or when the preceding component calls its <paramref name="next"/> delegate.
    /// </summary>
    Task Invoke(TContext context, PipelineInvocationDelegate<TContext> next, CancellationToken cancellationToken);
}
