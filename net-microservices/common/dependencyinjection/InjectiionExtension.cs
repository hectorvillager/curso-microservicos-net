namespace webapi.common.dependencyinjection;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInjectables(
        this IServiceCollection services, 
        params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        var injectableTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttribute<InjectableAttribute>() is not null
                        && type is { IsClass: true, IsAbstract: false });

        foreach (var implementationType in injectableTypes)
        {
            var attribute = implementationType.GetCustomAttribute<InjectableAttribute>()!;
            
            // Obtener todas las interfaces incluyendo las heredadas
            var allInterfaces = GetAllInterfaces(implementationType);
            
            foreach (var interfaceType in allInterfaces)
            {
                // Verificar si ya está registrado
                if (!IsServiceRegistered(services, interfaceType))
                {
                    RegisterService(services, interfaceType, implementationType, attribute.Lifetime);
                }
            }
            
            // Si no tiene interfaces, registrar la clase misma
            if (allInterfaces.Count == 0 && !IsServiceRegistered(services, implementationType))
            {
                RegisterService(services, implementationType, implementationType, attribute.Lifetime);
            }
        }

        return services;
    }

    private static HashSet<Type> GetAllInterfaces(Type type)
    {
        var interfaces = new HashSet<Type>();
        
        foreach (var interfaceType in type.GetInterfaces())
        {
            // Agregar la interfaz actual
            interfaces.Add(interfaceType);
            
            // Agregar todas las interfaces padre recursivamente
            foreach (var parentInterface in interfaceType.GetInterfaces())
            {
                interfaces.Add(parentInterface);
            }
        }
        
        return interfaces;
    }

    private static bool IsServiceRegistered(IServiceCollection services, Type serviceType)
    {
        return services.Any(descriptor => descriptor.ServiceType == serviceType);
    }

    private static void RegisterService(
        IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime)
    {
        var serviceDescriptor = lifetime switch
        {
            ServiceLifetime.Transient => 
                ServiceDescriptor.Transient(serviceType, implementationType),
            ServiceLifetime.Scoped => 
                ServiceDescriptor.Scoped(serviceType, implementationType),
            ServiceLifetime.Singleton => 
                ServiceDescriptor.Singleton(serviceType, implementationType),
            _ => throw new ArgumentOutOfRangeException(
                nameof(lifetime), 
                lifetime, 
                "Lifetime no válido")
        };

        services.Add(serviceDescriptor);
    }
}