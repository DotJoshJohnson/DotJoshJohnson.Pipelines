namespace DotJoshJohnson.Pipelines.Exceptions;

public class PipelineComponentNotFoundException<TComponent> : Exception
{
    public PipelineComponentNotFoundException()
        : base($"A pipeline component with the type \"{typeof(TComponent).FullName}\" could not be found.")
    { }
}
