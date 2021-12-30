namespace DotJoshJohnson.Pipelines.MicrosoftDependencyInjection;

using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

public class NamedTypeModule
{
    private const string _FallbackNamespace = "DotJoshJohnson.Pipelines";
    private const string _DynamicTypesSuffix = ".DynamicTypes";

    private readonly ModuleBuilder _moduleBuilder;
    private readonly ConcurrentDictionary<string, Type> _typeCache = new();
    private readonly string _baseNamespace = string.Empty;

    public static NamedTypeModule Instance { get; } = new();

    private NamedTypeModule()
    {
        _baseNamespace = string.Concat(GetType()?.Namespace ?? _FallbackNamespace, _DynamicTypesSuffix);

        _moduleBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new(_baseNamespace), AssemblyBuilderAccess.Run)
            .DefineDynamicModule(nameof(NamedTypeModule));
    }

    public Type GetOrCreate(string name)
    {
        name = string.Concat(_baseNamespace, ".", name);

        if (_typeCache.TryGetValue(name, out var type))
        {
            return type;
        }

        type = _moduleBuilder
            .DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed)
            .AsType();

        _typeCache[name] = type;

        return type;
    }
}
