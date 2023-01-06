namespace DotJoshJohnson.Pipelines.Components;

/// <summary>
/// The default <see cref="IPipelineComponentActivator"/> implementation used in <see cref="PipelineBuilder{TContext}"/> if a specific implementation is not provided.
/// Uses <see cref="Activator.CreateInstance{T}"/> to create new component instances.
/// </summary>
public class DefaultPipelineComponentActivator : IPipelineComponentActivator
{
    private DefaultPipelineComponentActivator()
    { }

    public static DefaultPipelineComponentActivator Instance { get; } = new();

    /// <inheritdoc/>
    public TComponent? Activate<TComponent, TContext>()
        where TComponent : class, IPipelineComponent<TContext>
    {
        return Activator.CreateInstance<TComponent>();
    }
}
