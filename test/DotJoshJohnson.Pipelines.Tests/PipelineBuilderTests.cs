namespace DotJoshJohnson.Pipelines.Tests;

using DotJoshJohnson.Pipelines.Components;

using System;

public class PipelineBuilderTests
{
    [Fact]
    public void WithActivator_ThrowsIfComponentsAlreadyAddedToPipeline()
    {
        var builder = new PipelineBuilder<PipelineContext>();

        builder.Use((context, cancellationToken, next) =>
        {
            return Task.CompletedTask;
        });

        Assert.Throws<InvalidOperationException>(() => builder.WithActivator(DefaultPipelineComponentActivator.Instance));
    }
}
