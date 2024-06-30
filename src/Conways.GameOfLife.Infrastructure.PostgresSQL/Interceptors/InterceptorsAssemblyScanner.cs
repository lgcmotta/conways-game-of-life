using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Conways.GameOfLife.Infrastructure.PostgresSQL.Interceptors;

public static class InterceptorsAssemblyScanner
{
    public static IEnumerable<IInterceptor> Scan(
        IServiceProvider? serviceProvider = null,
        params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            return [];
        }

        return assemblies
            .Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false } &&
                           type.IsAssignableTo(typeof(IInterceptor)))
            .Select(type =>
            {
                var constructor = type.GetConstructor(
                    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    types: Type.EmptyTypes,
                    modifiers: null
                );

                if (constructor is not null)
                {
                    return Activator.CreateInstance(type);
                }

                var instance = serviceProvider is not null
                    ? ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type)
                    : Activator.CreateInstance(type);

                return instance;
            })
            .Cast<IInterceptor>()
            .ToArray();
    }
}