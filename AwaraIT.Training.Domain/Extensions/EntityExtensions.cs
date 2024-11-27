using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.Training.Domain.Extensions
{
    public static class EntityExtensions
    {
        public static T GetAliasedValue<T>(this Entity entity, string attributeName)
        {
            if (!entity.Contains(attributeName))
                return default;

            var value = entity.GetAttributeValue<AliasedValue>(attributeName)?.Value;
            return (value != null) ? (T)value : default;
        }

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
