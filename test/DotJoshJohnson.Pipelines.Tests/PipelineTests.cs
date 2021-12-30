namespace DotJoshJohnson.Pipelines.Tests;

public class PipelineTests
{
    [Fact]
    public async Task Invoke_InvokesDelegatesInOrderAdded()
    {
        var context = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use(next => async (context, cancellationToken) =>
            {
                context.Data["test"] = string.Empty;
                context.Data["test"] += "d1";

                await next(context, cancellationToken);
            })
            .Use(next => async (context, cancellationToken) =>
            {
                context.Data["test"] += "d2";

                await next(context, cancellationToken);
            })
            .Use(next => (context, cancellationToken) =>
            {
                context.Data["test"] += "d3";

                return Task.CompletedTask;
            })
            .BuildAndInvoke(context);

        Assert.Equal("d1d2d3", context.Data["test"]);
    }

    [Fact]
    public async Task Invoke_InvokesComponentsInOrderAdded()
    {
        var context = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use<Component1>()
            .Use<Component2>()
            .Use<Component3>()
            .BuildAndInvoke(context);

        Assert.Equal("c1c2c3", context.Data["test"]);
    }

    [Fact]
    public async Task Invoke_InvokesDelegatesAndComponentsInOrderAdded()
    {
        var context = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use<Component1>()
            .Use(next => async (context, cancellationToken) =>
            {
                context.Data["test"] += "d1";

                await next(context, cancellationToken);
            })
            .Use(next => async (context, cancellationToken) =>
            {
                context.Data["test"] += "d2";

                await next(context, cancellationToken);
            })
            .Use<Component2>()
            .Use(next => async (context, cancellationToken) =>
            {
                context.Data["test"] += "d3";

                await next(context, cancellationToken);
            })
            .Use<Component3>()
            .BuildAndInvoke(context);

        Assert.Equal("c1d1d2c2d3c3", context.Data["test"]);
    }

    [Fact]
    public async Task Invoke_Succeeds_WithEmptyPipeline()
    {
        await new PipelineBuilder<PipelineContext>().BuildAndInvoke(new());
    }

    [Fact]
    public async Task Invoke_ThrowsExceptionFromInvokedDelegate()
    {
        await Assert.ThrowsAsync<BarrierPostPhaseException>(() => new PipelineBuilder<PipelineContext>()
            .Use(next => (context, cancellationToken) => throw new BarrierPostPhaseException())
            .BuildAndInvoke(new()));
    }

    [Fact]
    public async Task Invoke_ThrowsExceptionFromInvokedComponent()
    {
        await Assert.ThrowsAsync<BarrierPostPhaseException>(() => new PipelineBuilder<PipelineContext>()
            .Use<ThrowingComponent>()
            .BuildAndInvoke(new()));
    }

    class Component1 : IPipelineComponent<PipelineContext>
    {
        public async Task Invoke(PipelineContext context, PipelineInvocationDelegate<PipelineContext> next, CancellationToken cancellationToken)
        {
            context.Data["test"] = string.Empty;
            context.Data["test"] += "c1";

            await next(context, cancellationToken);
        }
    }

    class Component2 : IPipelineComponent<PipelineContext>
    {
        public async Task Invoke(PipelineContext context, PipelineInvocationDelegate<PipelineContext> next, CancellationToken cancellationToken)
        {
            context.Data["test"] += "c2";

            await next(context, cancellationToken);
        }
    }

    class Component3 : IPipelineComponent<PipelineContext>
    {
        public Task Invoke(PipelineContext context, PipelineInvocationDelegate<PipelineContext> next, CancellationToken cancellationToken)
        {
            context.Data["test"] += "c3";

            return Task.CompletedTask;
        }
    }

    class ThrowingComponent : IPipelineComponent<PipelineContext>
    {
        public Task Invoke(PipelineContext context, PipelineInvocationDelegate<PipelineContext> next, CancellationToken cancellationToken)
        {
            throw new BarrierPostPhaseException();
        }
    }
}
