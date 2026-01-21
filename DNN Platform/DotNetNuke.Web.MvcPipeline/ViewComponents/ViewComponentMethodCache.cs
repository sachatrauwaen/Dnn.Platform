// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline.ViewComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DotNetNuke.Services.Search.Entities;
    using DotNetNuke.Web.MvcPipeline.ViewComponents;
    using Microsoft.Extensions.DependencyInjection;

    // ViewComponentMethodCache.cs
    internal class ViewComponentMethodCache
    {
        private class CacheKey
        {
            public Type ComponentType { get; set; }
            public string[] ParameterNames { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is CacheKey other)
                {
                    return ComponentType == other.ComponentType &&
                           ParameterNames.SequenceEqual(other.ParameterNames);
                }
                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = ComponentType.GetHashCode();
                    foreach (var name in ParameterNames)
                    {
                        hash = hash * 397 ^ name.GetHashCode();
                    }
                    return hash;
                }
            }
        }

        public class CachedMethodInfo
        {
            public System.Reflection.MethodInfo Method { get; set; }
            public System.Reflection.ParameterInfo[] Parameters { get; set; }
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, List<CachedMethodInfo>>
            _methodsByType = new System.Collections.Concurrent.ConcurrentDictionary<Type, List<CachedMethodInfo>>();

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<CacheKey, CachedMethodInfo>
            _methodCache = new System.Collections.Concurrent.ConcurrentDictionary<CacheKey, CachedMethodInfo>();

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, System.Reflection.PropertyInfo[]>
            _propertyCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, System.Reflection.PropertyInfo[]>();

        public static CachedMethodInfo GetInvokeMethod(Type componentType, object arguments)
        {
            // Get all Invoke methods for this component type (cached)
            var methods = _methodsByType.GetOrAdd(componentType, type =>
            {
                return type.GetMethods()
                    .Where(m => m.Name == "Invoke" && m.ReturnType == typeof(IViewComponentResult))
                    .Select(m => new CachedMethodInfo
                    {
                        Method = m,
                        Parameters = m.GetParameters()
                    })
                    .OrderByDescending(m => m.Parameters.Length)
                    .ToList();
            });

            if (arguments == null)
            {
                var parameterlessMethod = methods.FirstOrDefault(m => m.Parameters.Length == 0);
                return parameterlessMethod ?? methods.FirstOrDefault();
            }

            // Get argument property names (cached by type)
            var argumentProperties = GetProperties(arguments.GetType());
            var parameterNames = argumentProperties.Select(p => p.Name.ToLowerInvariant()).OrderBy(n => n).ToArray();

            // Try to get cached method match
            var cacheKey = new CacheKey
            {
                ComponentType = componentType,
                ParameterNames = parameterNames
            };

            return _methodCache.GetOrAdd(cacheKey, key =>
            {
                // Find best matching method
                foreach (var cachedMethod in methods)
                {
                    if (IsMethodMatch(cachedMethod.Parameters, argumentProperties))
                    {
                        return cachedMethod;
                    }
                }

                return methods.FirstOrDefault();
            });
        }

        public static System.Reflection.PropertyInfo[] GetProperties(Type type)
        {
            return _propertyCache.GetOrAdd(type, t => t.GetProperties());
        }

        private static bool IsMethodMatch(System.Reflection.ParameterInfo[] parameters, System.Reflection.PropertyInfo[] argumentProperties)
        {
            var argumentDict = argumentProperties.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var param in parameters)
            {
                if (!argumentDict.ContainsKey(param.Name) && !param.IsOptional)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ClearCache()
        {
            _methodsByType.Clear();
            _methodCache.Clear();
            _propertyCache.Clear();
        }
    }
}

