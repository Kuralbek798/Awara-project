using System;
using System.Collections.Generic;
using System.Linq;

namespace AwaraIT.Training.Domain.Extensions
{
    /// <summary>
    /// Класс расширений для коллекций.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Проверяет, содержит ли коллекция элементы.
        /// </summary>
        /// <typeparam name="T">Тип элементов в коллекции.</typeparam>
        /// <param name="source">Исходная коллекция.</param>
        /// <returns>Значение <c>true</c>, если коллекция содержит элементы; в противном случае — <c>false</c>.</returns>
        public static bool HasItems<T>(this IEnumerable<T> source)
        {
            return source?.Any() ?? false;
        }

        /// <summary>
        /// Добавляет элементы из одной словарной коллекции в другую.
        /// </summary>
        /// <typeparam name="T">Тип ключей в словаре.</typeparam>
        /// <typeparam name="K">Тип значений в словаре.</typeparam>
        /// <param name="target">Целевая словарная коллекция.</param>
        /// <param name="source">Исходная словарная коллекция.</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="target"/> или <paramref name="source"/> равны <c>null</c>.</exception>
        public static void AddRange<T, K>(this IDictionary<T, K> target, IDictionary<T, K> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var element in source)
            {
                if (!target.ContainsKey(element.Key))
                    target.Add(element.Key, element.Value);
            }
        }
    }
}




