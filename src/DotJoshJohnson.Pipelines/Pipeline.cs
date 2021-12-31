namespace DotJoshJohnson.Pipelines;

/// <inheritdoc/>
public class Pipeline<TContext> : IPipeline<TContext>
{
    private readonly PipelineInvocationDelegate<TContext> _invocationDelegate;

    internal Pipeline(PipelineInvocationDelegate<TContext> invocationDelegate)
    {
        _invocationDelegate = invocationDelegate;
    }

    public async Task Invoke(TContext context, CancellationToken? cancellationToken = null)
    {
        cancellationToken = cancellationToken ?? CancellationToken.None;

        await _invocationDelegate.Invoke(context, cancellationToken.Value);
    }
}
