namespace Microsoft.Extensions.DependencyInjection;

using DotJoshJohnson.Pipelines;
using DotJoshJohnson.Pipelines.Components;
using DotJoshJohnson.Pipelines.MicrosoftDependencyInjection;

using Microsoft.Extensions.DependencyInjection.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds a configured pipeline to the container that can be accessed using <see cref="IPipeline{TContext}"/>.
    /// </summary>
    public static IServiceCollection AddPipeline<TContext>(this IServiceCollection services, Action<PipelineBuilder<TContext>> configurePipeline)
    {
        services.TryAddTransient<IPipelineComponentActivator, MSDIPipelineComponentActivator>();
        services.AddTransient(serviceProvider => _BuildPipeline(serviceProvider, configurePipeline));

        return services;
    }

    /// <summary>
    /// Adds a configured named pipeline to the container that can be accessed using <see cref="INamedPipeline.Get{TContext}(string)"/>.
    /// </summary>
    public static IServiceCollection AddNamedPipeline<TContext>(this IServiceCollection services, string name, Action<PipelineBuilder<TContext>> configurePipeline)
    {
        var namedType = NamedTypeModule.Instance.GetOrCreate(name);

        services.TryAddTransient<IPipelineComponentActivator, MSDIPipelineComponentActivator>();
        services.TryAddSingleton<INamedPipeline, MSDINamedPipeline>();

        services.AddTransient(namedType, serviceProvider => _BuildPipeline(serviceProvider, configurePipeline));

        return services;
    }

    private static IPipeline<TContext> _BuildPipeline<TContext>(IServiceProvider serviceProvider, Action<PipelineBuilder<TContext>> configurePipeline)
    {
        var pipelineComponentActivator = serviceProvider.GetRequiredService<IPipelineComponentActivator>();
        var pipelineBuilder = new PipelineBuilder<TContext>().WithActivator(pipelineComponentActivator);

        configurePipeline(pipelineBuilder);

        return pipelineBuilder.Build();
    }
}
