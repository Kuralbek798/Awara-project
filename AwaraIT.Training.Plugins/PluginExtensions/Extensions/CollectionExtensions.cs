﻿using AwaraIT.Kuralbek.Plugins.PluginExtensions.Attributes;
using System.Collections.Generic;

namespace AwaraIT.Kuralbek.Plugins.PluginExtensions.Extensions
{
    public static class CollectionExtensions
    {
        public static TValue GetValueOrDefault<TValue>(
            this Dictionary<string, string> collection,
            string key,
            TValue defaultValue)
        {
            Argument.NotNull((object)collection, "Collection is required.");
            TValue obj;
            if (collection.ContainsKey(key) && collection[key].TryParse<TValue>(out obj))
                return obj;
            return defaultValue;
        }
    }
}
