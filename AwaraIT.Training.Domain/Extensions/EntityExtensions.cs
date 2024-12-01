using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.Training.Domain.Extensions
{
    /// <summary>
    /// Класс расширений для сущностей CRM.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Получает значение атрибута, который является алиасом, и преобразует его в указанный тип.
        /// </summary>
        /// <typeparam name="T">Тип значения атрибута.</typeparam>
        /// <param name="entity">Сущность CRM.</param>
        /// <param name="attributeName">Имя атрибута.</param>
        /// <returns>Значение атрибута, преобразованное в указанный тип, или значение по умолчанию, если атрибут не найден.</returns>
        public static T GetAliasedValue<T>(this Entity entity, string attributeName)
        {
            if (!entity.Contains(attributeName))
                return default;

            var value = entity.GetAttributeValue<AliasedValue>(attributeName)?.Value;
            return (value != null) ? (T)value : default;
        }

        /// <summary>
        /// Получает значение атрибута, который является алиасом, и преобразует его в указанный тип.
        /// </summary>
        /// <param name="entity">Сущность CRM.</param>
        /// <param name="attributeName">Имя атрибута.</param>
        /// <param name="type">Тип значения атрибута.</param>
        /// <returns>Значение атрибута, преобразованное в указанный тип, или значение по умолчанию, если атрибут не найден.</returns>
        /// <exception cref="ArgumentNullException">Если параметр <paramref name="type"/> равен null.</exception>
        public static object GetAliasedValue(this Entity entity, string attributeName, Type type)
        {
            if (!entity.Contains(attributeName))
            {
                return default;
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var value = entity.GetAttributeValue<AliasedValue>(attributeName)?.Value;

            return (value != null) ? Convert.ChangeType(value, type) : default;
        }

        /// <summary>
        /// Получает значение атрибута из сущности или из изображения сущности.
        /// </summary>
        /// <typeparam name="T">Тип значения атрибута.</typeparam>
        /// <param name="entity">Сущность CRM.</param>
        /// <param name="attributeName">Имя атрибута.</param>
        /// <param name="image">Изображение сущности.</param>
        /// <returns>Значение атрибута, преобразованное в указанный тип, или значение по умолчанию, если атрибут не найден.</returns>
        public static T GetAttributeValueImage<T>(this Entity entity, string attributeName, Entity image)
        {
            var result = default(T);
            if (entity != null && entity.Contains(attributeName))
            {
                result = entity.GetAttributeValue<T>(attributeName);
            }
            else if (image != null && image.Contains(attributeName))
            {
                result = image.GetAttributeValue<T>(attributeName);
            }

            return result;
        }
    }
}




