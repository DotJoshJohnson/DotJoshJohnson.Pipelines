namespace DotJoshJohnson.Pipelines;

/// <summary>
/// Provides access to named pipelines.
/// </summary>
public interface INamedPipeline
{
    /// <summary>
    /// Gets the specified named pipeline if available.
    /// </summary>
    IPipeline<TContext>? Get<TContext>(string name);
}
