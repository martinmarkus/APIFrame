using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace APIFrame.BackgroundService.WireUp
{
    public static class ServiceCollectionExtension
    {
        public static void AddJobConfigurations(
            this IServiceCollection services,
            string configBasePath)
        {
            var resultTypes = new List<Type>();

            var entryTypes = Assembly.GetEntryAssembly().GetTypes();
            resultTypes.AddRange(entryTypes);

            var referencedAssemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var types = Assembly.Load(assemblyName).GetTypes();
                resultTypes.AddRange(types);
            }

            var files = Directory.GetFiles(configBasePath);
            foreach (var filePath in files)
            {
                try
                {
                    using var reader = new StreamReader(filePath);
                    var json = reader.ReadToEnd();

                    var implementationTypeName = Path.GetFileNameWithoutExtension(filePath);

                    foreach (var type in resultTypes)
                    {
                        if (type.Name.Equals(implementationTypeName, StringComparison.OrdinalIgnoreCase))
                        {
                            var result = JsonConvert.DeserializeObject(json, type);
                            services.AddSingleton(type, result);
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
