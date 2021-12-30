namespace DotJoshJohnson.Pipelines.Tests;

using System;

public class PipelineBuilderTests
{
    [Fact]
    public void WithActivator_ThrowsIfComponentsAlreadyAddedToPipeline()
    {
        var builder = new PipelineBuilder<PipelineContext>();

        builder.Use(next => (context, cancellationToken) =>
        {
            return Task.CompletedTask;
        });

        Assert.Throws<InvalidOperationException>(() => builder.WithActivator(DefaultPipelineComponentActivator.Instance));
    }
}
