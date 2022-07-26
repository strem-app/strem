﻿using Strem.Core.Services.Registries;

namespace Strem.Core.Extensions;

public static class IRegistryExtensions
{
    public static void AddMany<T>(this IRegistry<T> registry, IEnumerable<T> entries) where T : class => entries.ForEach(registry.Add);
    public static void RemoveMany<T>(this IRegistry<T> registry, IEnumerable<T> entries) where T : class => entries.ForEach(registry.Remove);
}