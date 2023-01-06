using DotJoshJohnson.Pipelines.Components;
using DotJoshJohnson.Pipelines.Events;

namespace DotJoshJohnson.Pipelines.Tests;

public class PipelineTests
{
    [Fact]
    public async Task Invoke_InvokesDelegatesInOrderAdded()
    {
        var context = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use(async (context, cancellationToken, next) =>
            {
                context.Data["test"] = string.Empty;
                context.Data["test"] += "d1";

                await next(context, cancellationToken);
            })
            .Use(async (context, cancellationToken, next) =>
            {
                context.Data["test"] += "d2";

                await next(context, cancellationToken);
            })
            .Use((context, cancellationToken, next) =>
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
            .Use(async (context, cancellationToken, next) =>
            {
                context.Data["test"] += "d1";

                await next(context, cancellationToken);
            })
            .Use(async (context, cancellationToken, next) =>
            {
                context.Data["test"] += "d2";

                await next(context, cancellationToken);
            })
            .Use<Component2>()
            .Use(async (context, cancellationToken, next) =>
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
            .Use((context, cancellationToken, next) => throw new BarrierPostPhaseException())
            .BuildAndInvoke(new()));
    }

    [Fact]
    public async Task Invoke_ThrowsExceptionFromInvokedComponent()
    {
        await Assert.ThrowsAsync<BarrierPostPhaseException>(() => new PipelineBuilder<PipelineContext>()
            .Use<ThrowingComponent>()
            .BuildAndInvoke(new()));
    }

    [Fact]
    public async Task Invoke_RaisesBeforeComponentInvokedEvents()
    {
        var pipelineContext = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use((context, cancellationToken, next) =>
            {
                return Task.CompletedTask;
            })
            .AddEventHandler((context, cancellationToken) =>
            {
                context.PipelineContext.Data["event-type"] = context.EventType;

                return Task.CompletedTask;
            }, PipelineEventType.BeforeComponentInvoked)
            .BuildAndInvoke(pipelineContext);

        Assert.Equal(PipelineEventType.BeforeComponentInvoked, pipelineContext.Data["event-type"]);
    }

    [Fact]
    public async Task Invoke_RaisesAfterComponentSucceededEvents()
    {
        var pipelineContext = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use((context, cancellationToken, next) =>
            {
                return Task.CompletedTask;
            })
            .AddEventHandler((context, cancellationToken) =>
            {
                context.PipelineContext.Data["event-type"] = context.EventType;

                return Task.CompletedTask;
            }, PipelineEventType.AfterComponentSucceeded)
            .BuildAndInvoke(pipelineContext);

        Assert.Equal(PipelineEventType.AfterComponentSucceeded, pipelineContext.Data["event-type"]);
    }

    [Fact]
    public async Task Invoke_RaisesAfterComponentFailedEvents()
    {
        var pipelineContext = new PipelineContext();

        try
        {
            await new PipelineBuilder<PipelineContext>()
                .Use((context, cancellationToken, next) =>
                {
                    throw new System.Exception();
                })
                .AddEventHandler((context, cancellationToken) =>
                {
                    context.PipelineContext.Data["event-type"] = context.EventType;

                    return Task.CompletedTask;
                }, PipelineEventType.AfterComponentFailed)
                .BuildAndInvoke(pipelineContext);
        }

        catch { }
        

        Assert.Equal(PipelineEventType.AfterComponentFailed, pipelineContext.Data["event-type"]);
    }

    [Fact]
    public async Task Invoke_RaisesAfterComponentInvokedEvents()
    {
        var pipelineContext = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use((context, cancellationToken, next) =>
            {
                return Task.CompletedTask;
            })
            .AddEventHandler((context, cancellationToken) =>
            {
                context.PipelineContext.Data["event-type"] = context.EventType;

                return Task.CompletedTask;
            }, PipelineEventType.AfterComponentInvoked)
            .BuildAndInvoke(pipelineContext);

        Assert.Equal(PipelineEventType.AfterComponentInvoked, pipelineContext.Data["event-type"]);
    }

    [Fact]
    public async Task Invoke_RaisesAllEvents()
    {
        var pipelineContext = new PipelineContext();

        await new PipelineBuilder<PipelineContext>()
            .Use((context, cancellationToken, next) =>
            {
                return Task.CompletedTask;
            })
            .AddEventHandler((context, cancellationToken) =>
            {
                if (!context.PipelineContext.Data.ContainsKey("event-types"))
                {
                    context.PipelineContext.Data["event-types"] = string.Empty;
                }

                context.PipelineContext.Data["event-types"] = $"{context.PipelineContext.Data["event-types"]}|{context.EventType}";

                return Task.CompletedTask;
            })
            .BuildAndInvoke(pipelineContext);

        Assert.Equal("|BeforeComponentInvoked|AfterComponentSucceeded|AfterComponentInvoked", pipelineContext.Data["event-types"]);
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
