namespace DotJoshJohnson.Pipelines.MicrosoftDependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public class MSDIPipelineComponentActivator : IPipelineComponentActivator
{
    private readonly IServiceProvider _serviceProvider;

    public MSDIPipelineComponentActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TComponent Activate<TComponent, TContext>()
        where TComponent : class, IPipelineComponent<TContext>
    {
        return _serviceProvider.GetRequiredService<TComponent>();
    }
}
