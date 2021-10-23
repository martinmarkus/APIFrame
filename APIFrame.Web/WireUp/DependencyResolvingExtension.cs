using APIFrame.Core.Exceptions;
using APIFrame.Web.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APIFrame.Web.WireUp
{
    public static class DependencyResolvingExtension
    {
        /// <summary>
        /// Dynamically resolves every interface-implementation pairs in the given namespaces.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="interfaceNs"></param>
        /// <param name="impNs"></param>
        public static void ResolveDynamically(this IServiceCollection services, string interfaceNs, string impNs)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(type => type.GetTypes())
                .Where(type => !string.IsNullOrEmpty(type.Namespace));

            var interfaces = types
                .Where(type => type.IsInterface && type.Namespace.Equals(interfaceNs, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var implementations = types
                .Where(type => type.IsClass && type.Namespace.Equals(impNs, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                try
                {
                    var implementationType = implementations.FirstOrDefault(implementation =>
                            implementation.Name.Equals(interfaceType.Name[1..], StringComparison.OrdinalIgnoreCase));

                    if (implementationType == null)
                    {
                        throw new MissingDependencyImplementationException(interfaceType.FullName, impNs);
                    }

                    if (IsExceptedFromDynamicResolve(interfaceType, implementationType))
                    {
                        break;
                    }

                    services.AddScoped(interfaceType, implementationType);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Resolves dynamically every possible interface-implementation pairs in the given assembly.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        public static void ResolveDynamically(this IServiceCollection services, Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();

            var interfaces = new List<Type>();
            var implementations = new List<Type>();

            foreach (var type in assemblyTypes)
            {
                if (string.IsNullOrEmpty(type.Namespace))
                {
                    continue;
                }
                else if (type.IsInterface)
                {
                    interfaces.Add(type);
                }
                else if (type.IsClass)
                {
                    implementations.Add(type);
                }
            }

            foreach (var implementationType in implementations)
            {
                foreach (var interfaceType in interfaces)
                {
                    try
                    {
                        if (implementationType.Name.Equals(interfaceType.Name[1..], StringComparison.OrdinalIgnoreCase))
                        {
                            if (IsExceptedFromDynamicResolve(interfaceType, implementationType))
                            {
                                break;   
                            }

                            services.AddScoped(interfaceType, implementationType);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        private static bool IsExceptedFromDynamicResolve(Type interfaceType, Type implementationType)
        {
            var interfaceExceptAttribute = interfaceType.GetCustomAttribute(typeof(ExceptDynamicResolve));
            var impExceptAttribute = implementationType.GetCustomAttribute(typeof(ExceptDynamicResolve));

            return impExceptAttribute != null || interfaceExceptAttribute != null;
        }
    }
}
