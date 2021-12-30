namespace DotJoshJohnson.Pipelines.MicrosoftDependencyInjection;

public class MSDINamedPipeline : INamedPipeline
{
    private readonly IServiceProvider _serviceProvider;

    public MSDINamedPipeline(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPipeline<TContext>? Get<TContext>(string name)
    {
        var namedType = NamedTypeModule.Instance.GetOrCreate(name);

        var instance = _serviceProvider.GetService(namedType);

        return instance as IPipeline<TContext>;
    }
}
