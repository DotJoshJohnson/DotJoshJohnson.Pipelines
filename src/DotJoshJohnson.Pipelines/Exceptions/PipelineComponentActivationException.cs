namespace DotJoshJohnson.Pipelines.Exceptions;

public class PipelineComponentActivationException<TComponent> : Exception
{
    public PipelineComponentActivationException()
        : base($"Failed to activate \"{typeof(TComponent).FullName}\". If you're using DI, make sure you've registered your component with the container.")
    { }

    public PipelineComponentActivationException(Exception ex)
        : base($"Failed to activate \"{typeof(TComponent).FullName}\". If you're using DI, make sure you've registered your component with the container.", ex)
    { }
}
