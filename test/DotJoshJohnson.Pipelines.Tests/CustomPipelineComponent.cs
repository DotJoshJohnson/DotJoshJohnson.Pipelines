namespace DotJoshJohnson.Pipelines.Tests;

public class CustomPipelineComponent : IPipelineComponent<PipelineContext>
{
    public Task Invoke(PipelineContext context, PipelineInvocationDelegate<PipelineContext> next, CancellationToken cancellationToken)
    {
        context.Data["test"] += "a";

        return Task.CompletedTask;
    }
}
