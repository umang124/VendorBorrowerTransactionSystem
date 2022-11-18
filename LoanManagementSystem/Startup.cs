using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LoanManagementSystem
{
    public static class Startup
    {
        private const string DefaultControllerFilter = "*Controller";
        public static void AddMvcControllers(this IServiceCollection serviceCollection, params string[] assemblyFilters)
        {
            serviceCollection.AddMvcControllers(GetAssemblies(assemblyFilters));
        }
        private static Assembly[] GetAssemblies(IEnumerable<string> assemblyFilters)
        {
            var assemblies = new List<Assembly>();
            foreach (var assemblyFilter in assemblyFilters)
            {
                assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(assembly => IsWildcardMatch(assembly.GetName().Name, assemblyFilter)).ToArray());
            }
            return assemblies.ToArray();
        }

        private static bool IsWildcardMatch(string input, string wildcard)
        {
            return input == wildcard || Regex.IsMatch(input, "^" + Regex.Escape(wildcard).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
        }
        public static void AddMvcControllers(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            serviceCollection.AddMvcControllers(assemblies, new[] { DefaultControllerFilter });
        }

        private static void AddMvcControllers(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies, string[] classFilters)
        {
            var controllers = GetTypesImplementing(typeof(IController), assemblies, classFilters);

            foreach (var controller in controllers)
            {
                serviceCollection.Add(controller, Lifetime.Transient);
            }
        }

        public static void Add(this IServiceCollection serviceCollection, Type type, Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    serviceCollection.AddSingleton(type);
                    break;
                case Lifetime.Transient:
                    serviceCollection.AddTransient(type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        }

        private static IEnumerable<Type> GetTypesImplementing(Type implementsType, IEnumerable<Assembly> assemblies, params string[] classFilter)
        {
            var types = GetTypesImplementing(implementsType, assemblies.ToArray());
            if (classFilter != null && classFilter.Any())
            {
                types = types.Where(type => classFilter.Any(filter => IsWildcardMatch(type.FullName, filter)));
            }
            return types;
        }

        private static IEnumerable<Type> GetTypesImplementing(Type implementsType, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                return new Type[0];
            }

            var targetType = implementsType;

            return assemblies
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetExportedTypes)
                .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition && targetType.IsAssignableFrom(type))
                .ToArray();
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException)
            {
                // A type load exception would typically happen on an Anonymously Hosted DynamicMethods
                // Assembly and it would be safe to skip this exception.
                return Type.EmptyTypes;
            }
            catch (FileLoadException)
            {
                // The assembly points to a not found assembly - ignore and continue
                return Type.EmptyTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Return the types that could be loaded. Types can contain null values.
                return ex.Types.Where(type => type != null);
            }
            catch (Exception ex)
            {
                // Throw a more descriptive message containing the name of the assembly.
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to load types from assembly {0}. {1}", assembly.FullName, ex.Message), ex);
            }
        }
    }

    public enum Lifetime
    {
        Transient,
        Singleton
    }
}