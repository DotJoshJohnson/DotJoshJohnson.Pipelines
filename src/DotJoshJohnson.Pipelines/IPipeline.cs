namespace DotJoshJohnson.Pipelines;

/// <summary>
/// The compiled pipeline instance, ready to be invoked.
/// </summary>
public interface IPipeline<TContext>
{
    /// <summary>
    /// Invokes the configured pipeline delegates and components in the order they were added to the pipeline.
    /// </summary>
    Task Invoke(TContext context, CancellationToken? cancellationToken = default);
}
