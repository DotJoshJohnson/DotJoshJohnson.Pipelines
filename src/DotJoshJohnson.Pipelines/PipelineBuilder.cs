namespace DotJoshJohnson.Pipelines;

using DotJoshJohnson.Pipelines.Exceptions;

/// <summary>
/// Responsible for configuring and compiling pipelines for invocation.
/// </summary>
public class PipelineBuilder<TContext>
{
    private readonly List<PipelineConfigurationDelegate<TContext>> _configurationDelegates = new();

    private IPipelineComponentActivator _pipelineComponentActivator = DefaultPipelineComponentActivator.Instance;

    public IReadOnlyList<PipelineConfigurationDelegate<TContext>> ConfigurationDelegates => _configurationDelegates.AsReadOnly();

    /// <summary>
    /// Compiles and returns a new pipeline ready for invocation.
    /// </summary>
    /// <returns></returns>
    public IPipeline<TContext> Build()
    {
        var root = new PipelineInvocationDelegate<TContext>((context, cancellationToken) => Task.CompletedTask);

        for (var i = _configurationDelegates.Count - 1; i >= 0; i--)
        {
            var configureDelegate = _configurationDelegates[i];

            root = configureDelegate(root);
        }

        return new Pipeline<TContext>(root);
    }

    /// <summary>
    /// Compiles and invokes a new pipeline.
    /// </summary>
    public async Task BuildAndInvoke(TContext context, CancellationToken? cancellationToken = default)
    {
        var pipeline = Build();

        await pipeline.Invoke(context, cancellationToken);
    }

    /// <summary>
    /// Adds the provided delegate to the pipeline.
    /// </summary>
    public PipelineBuilder<TContext> Use(PipelineConfigurationDelegate<TContext> configureHandler)
    {
        _configurationDelegates.Add(configureHandler);

        return this;
    }

    /// <summary>
    /// Adds the specified component to the pipeline.
    /// </summary>
    public PipelineBuilder<TContext> Use<TComponent>()
        where TComponent : class, IPipelineComponent<TContext>
    {
        return Use(next => new PipelineInvocationDelegate<TContext>((context, cancellationToken) =>
        {
            TComponent? component = default;

            try
            {
                component = _pipelineComponentActivator.Activate<TComponent, TContext>();
            }

            catch (Exception ex)
            {
                throw new PipelineComponentActivationException<TComponent>(ex);
            }

            if (component is null)
            {
                throw new PipelineComponentActivationException<TComponent>();
            }

            return component.Invoke(context, next, cancellationToken);
        }));
    }

    /// <summary>
    /// Sets the pipeline's component activator implementation.
    /// This method can be called more than once to override the current activator, but must be called before any components are added to the pipeline.
    /// </summary>
    public PipelineBuilder<TContext> WithActivator(IPipelineComponentActivator pipelineComponentActivator)
    {
        if (_configurationDelegates.Any())
        {
            throw new InvalidOperationException("WithActivator must be called before any components are added to the pipeline builder.");
        }

        _pipelineComponentActivator = pipelineComponentActivator;

        return this;
    }
}
