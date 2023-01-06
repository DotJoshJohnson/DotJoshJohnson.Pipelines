namespace DotJoshJohnson.Pipelines.MicrosoftDependencyInjection.Tests;

public class IServiceCollectionExtensionsTests
{
    [Fact]
    public async void AddPipeline_AddsPipelineToDIContainer()
    {
        var serviceProvider = new ServiceCollection()
            .AddPipeline<PipelineContext>(p =>
            {
                p.Use((context, cancellationToken, next) => Task.CompletedTask);
            })
            .BuildServiceProvider();

        var pipeline = serviceProvider.GetRequiredService<IPipeline<PipelineContext>>();

        Assert.IsAssignableFrom<IPipeline<PipelineContext>>(pipeline);
        Assert.NotNull(pipeline);

        await pipeline.Invoke(new());
    }

    [Fact]
    public async void AddNamedPipeline_AddsNamedPipelineToDIContainer()
    {
        var serviceProvider = new ServiceCollection()
            .AddNamedPipeline<PipelineContext>("test1", p =>
            {
                p.Use((context, cancellationToken, next) => Task.CompletedTask);
            })
            .AddNamedPipeline<PipelineContext>("test2", p =>
            {
                p.Use((context, cancellationToken, next) => Task.CompletedTask);
            })
            .BuildServiceProvider();

        var pipeline = serviceProvider.GetRequiredService<INamedPipeline>().Get<PipelineContext>("test1");

        Assert.IsAssignableFrom<IPipeline<PipelineContext>>(pipeline);
        Assert.NotNull(pipeline);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await pipeline.Invoke(new());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}