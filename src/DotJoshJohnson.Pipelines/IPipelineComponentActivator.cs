namespace DotJoshJohnson.Pipelines;

/// <summary>
/// Responsible for providing an instance of the specified pipeline component during pipeline invocation.
/// </summary>
public interface IPipelineComponentActivator
{
    /// <summary>
    /// Called by the pipeline when an instance of the specified component is required.
    /// </summary>
    TComponent? Activate<TComponent, TContext>()
        where TComponent : class, IPipelineComponent<TContext>;
}
