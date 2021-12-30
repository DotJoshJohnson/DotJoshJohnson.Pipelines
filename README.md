# DotJoshJohnson.Pipelines
A simple utility library for building and invoking pipelines similar to the ASP.NET Core middleware pipeline.

## Getting Started

### Quick and Dirty

1. Install `DotJoshJohnson.Pipelines` via NuGet.
2. Build a pipeline.
```csharp
using DotJoshJohnson.Pipelines;
  
// ... //
  
var pipeline = new PipelineBuilder<PipelineContext>()
    .Use(next => async (context, cancellationToken) =>
    {
        // TODO: do work
          
        await next(context, cancellationToken);
    })
    .Use(next => async (context, cancellationToken) =>
    {
        // TODO: do more work
          
        await next(context, cancellationToken);
    })
    .Build();
```
4. Invoke your pipeline!
```csharp
// continued from step 2 //

var context = new PipelineContext();

await pipeline.Invoke(context);
```

### Using Pipeline Components

Instead of adding delegates directly to your pipeline, you can add components, which are classes that implement `IPipelineComponent<TContext>`.

```csharp
using DotJoshJohnson.Pipelines;

// ... //

public class MyCustomComponent : IPipelineComponent<PipelineContext>
{
    public async Task Invoke(PipelineContext context, PipelineInvocationDelegate<PipelineContext> next, CancellationToken cancellationToken)
    {
        // TODO: do work here
        
        await next(context, cancellationToken);
        
        // TODO: maybe do work here as well
    }
}

// ... //

var pipeline = new PipelineBuilder<PipelineContext>()
    .Use<MyCustomComponent>()
    .Build();
```

When it comes time to invoke your component, it will be "activated", which just means an instance of that class will be requested by the pipeline. The default activator just uses `Activator.CreateInstance<TComponent>()` to get a new instance of your component. You can implement your own activator (`IPipelineComponentActivator`) if you'd like or use an extension such as `DotJoshJohnson.Pipelines.MicrosoftDependencyInjection` that provides one for you. If you're writing your own activator, be sure to tell the builder about it by calling `WithActivator()` on your pipeline builder. Note that this method must be called before any components are added ot the pipeline.

### Custom Pipeline Context

The `PipelineContext` object you've seen thus far is simply included for convenience. You can use it as-is, extend it via inheritance, or use a totally different class. There is no requirement to use or extend `PipelineContext`.

### Dependency Injection

If you are using `Microsoft.Extensions.DependencyInjection`, I recommend pulling in `DotJoshJohnson.Pipelines.MicrosoftDependencyInjection`. This package provides two extensions to `IServiceCollection`. **Note that if you are using pipeline components, you must register each component with the DI container.**

**Using AddPipeline**

```csharp
using DotJoshJohnson.Pipelines;
using Microsoft.Extensions.DependencyInjection;

// ... //

var services = new ServiceCollection();

services.AddPipeline<PipelineContext>(p =>
{
    p.Use(next => async (context, cancellationToken) =>
    {
        // TODO: do work!
        
        await next(context, cancellationToken);
    });
    
    p.Use<MyCustomComponent>();
});

var serviceProvider = services.BuildServiceProvider();

var pipeline = serviceProvider.GetRequiredService<IPipeline<PipelineContext>>();

await pipeline.Invoke(new());
```

**Using AddNamedPipeline**

```csharp
using DotJoshJohnson.Pipelines;
using Microsoft.Extensions.DependencyInjection;

// ... //

var services = new ServiceCollection();

services.AddNamedPipeline<PipelineContext>("MyPipeline", p =>
{
    p.Use(next => async (context, cancellationToken) =>
    {
        // TODO: do work!
        
        await next(context, cancellationToken);
    });
    
    p.Use<MyCustomComponent>();
});

var serviceProvider = services.BuildServiceProvider();

var pipeline = serviceProvider.GetRequiredService<INamedPipeline>().Get<PipelineContext>("MyPipeline");

await pipeline.Invoke(new());
```
