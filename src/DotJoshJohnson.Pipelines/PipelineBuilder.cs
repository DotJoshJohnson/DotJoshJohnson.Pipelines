namespace DotJoshJohnson.Pipelines;

using DotJoshJohnson.Pipelines.Components;
using DotJoshJohnson.Pipelines.Events;
using DotJoshJohnson.Pipelines.Exceptions;

/// <summary>
/// Responsible for configuring and compiling pipelines for invocation.
/// </summary>
public class PipelineBuilder<TContext>
{
    private readonly List<PipelineConfigurationDelegate<TContext>> _configurationDelegates = new();
    private readonly List<(PipelineEventType? eventType, PipelineEventHandlerDelegate<TContext> handler)> _eventHandlers = new();

    private IPipelineComponentActivator _pipelineComponentActivator = DefaultPipelineComponentActivator.Instance;

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
    internal PipelineBuilder<TContext> Use(PipelineConfigurationDelegate<TContext> configureHandler)
    {
        _configurationDelegates.Add(configureHandler);

        return this;
    }

    /// <summary>
    /// Adds the provided delegate to the pipeline.
    /// </summary>
    public PipelineBuilder<TContext> Use(Func<TContext, CancellationToken, PipelineInvocationDelegate<TContext>, Task> handler)
    {
        return Use(next => new PipelineInvocationDelegate<TContext>(async (context, cancellationToken) =>
        {
            var eventContext = new PipelineEventContext<TContext>(PipelineEventType.BeforeComponentInvoked)
            {
                ComponentType = handler.GetType(),
                PipelineContext = context
            };

            await _InvokeEventHandlers(eventContext, cancellationToken);

            try
            {
                await handler(context, cancellationToken, next);

                await _InvokeEventHandlers(eventContext.WithEventType(PipelineEventType.AfterComponentSucceeded), cancellationToken);
            }

            catch (Exception ex)
            {
                await _InvokeEventHandlers(eventContext.WithEventType(PipelineEventType.AfterComponentFailed), cancellationToken);

                throw;
            }

            finally
            {
                await _InvokeEventHandlers(eventContext.WithEventType(PipelineEventType.AfterComponentInvoked), cancellationToken);
            }
        }));
    }

    /// <summary>
    /// Adds the specified component to the pipeline.
    /// </summary>
    public PipelineBuilder<TContext> Use<TComponent>()
        where TComponent : class, IPipelineComponent<TContext>
    {
        return Use(next => new PipelineInvocationDelegate<TContext>(async (context, cancellationToken) =>
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

            var eventContext = new PipelineEventContext<TContext>(PipelineEventType.BeforeComponentInvoked)
            {
                ComponentInstance = component,
                ComponentType = component.GetType(),
                PipelineContext = context
            };

            await _InvokeEventHandlers(eventContext, cancellationToken);

            try
            {
                await component.Invoke(context, next, cancellationToken);

                await _InvokeEventHandlers(eventContext.WithEventType(PipelineEventType.AfterComponentSucceeded), cancellationToken);
            }

            catch (Exception ex)
            {
                await _InvokeEventHandlers(eventContext.WithEventType(PipelineEventType.AfterComponentFailed), cancellationToken);

                throw;
            }

            finally
            {
                await _InvokeEventHandlers(eventContext.WithEventType(PipelineEventType.AfterComponentInvoked), cancellationToken);
            }
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

    /// <summary>
    /// Adds the provided pipeline event handler to the pipeline.
    /// An event type can be provided to ensure the handler is only invoked for that event type.
    /// </summary>
    public PipelineBuilder<TContext> AddEventHandler(PipelineEventHandlerDelegate<TContext> handleEvent, PipelineEventType? eventType = null)
    {
        _eventHandlers.Add((eventType, handleEvent));

        return this;
    }

    private async Task _InvokeEventHandlers(PipelineEventContext<TContext> eventContext, CancellationToken cancellationToken)
    {
        var handlers = _eventHandlers.Where(h => h.eventType is null || h.eventType == eventContext.EventType).Select(h => h.handler).ToArray();

        foreach (var handler in handlers)
        {
            await handler(eventContext, cancellationToken);
        }
    }
}
