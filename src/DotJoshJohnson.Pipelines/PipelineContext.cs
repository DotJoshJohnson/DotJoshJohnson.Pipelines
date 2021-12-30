namespace DotJoshJohnson.Pipelines;

/// <summary>
/// A simple default context object that can be used as-is or extended via inheritance.
/// Note that you are *not* required to use or extend this as your context type. It is simply provided for convenience.
/// </summary>
public class PipelineContext
{
    public string CorrelationId { get; } = Guid.NewGuid().ToString();
    public Dictionary<string, object> Data { get; } = new();

    public T? GetDataAs<T>(string key)
    {
        if (Data.TryGetValue(key, out var value) && value is not null)
        {
            return (T)value;
        }

        return default;
    }
}
