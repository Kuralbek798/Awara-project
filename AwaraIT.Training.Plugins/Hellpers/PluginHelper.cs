using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System;
using AwaraIT.Training.Application.Core;
using System.Linq;
using AwaraIT.Training.Domain.Models.Crm;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using AwaraIT.Training.Domain.Extensions;
using AwaraIT.Kuralbek.Plugins.Plugin;
using System.Runtime.ExceptionServices;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Helpers
{
    public static class PluginHelper
    {
        /// <summary>
        /// Получает сущность с наименьшей нагрузкой на основе заданных условий.
        /// </summary>
        /// <param name="wrapper">Экземпляр IContextWrapper для выполнения запроса.</param>
        /// <param name="conditionExpressions">Список условий для фильтрации записей.</param>
        /// <param name="entityLogicalName">Логическое имя сущности для запроса.</param>
        /// <param name="ownerAttributeName">Имя атрибута владельца в сущности.</param>
        /// <param name="logger">Экземпляр Logger для логирования.</param>
        /// <returns>Сущность с наименьшей нагрузкой.</returns>
        /// <exception cref="InvalidPluginExecutionException">Выбрасывается при возникновении ошибки во время выполнения запроса.</exception>
        public static Entity GetLeastLoadedEntity(
            IContextWrapper wrapper,
            List<ConditionExpression> conditionExpressions,
            string entityLogicalName,
            string ownerAttributeName,
            Logger logger)
        {
            Logger log = logger;
            try
            {
                // Создаем запрос
                var loadQuery = new QueryExpression(entityLogicalName)
                {
                    ColumnSet = new ColumnSet(ownerAttributeName),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                    }
                };

                // Добавляем условия в запрос
                conditionExpressions.ForEach(condition => loadQuery.Criteria.AddCondition(condition));

                // Делаем запрос
                var entityRecords = wrapper.Service.RetrieveMultiple(loadQuery).Entities;

                if (!entityRecords.Any())
                {
                    log.WARNING("No records found matching the specified conditions.");
                    return new Entity();
                }

                // Считаем количество записей для каждого пользователя
                var userLoadCounts = entityRecords
                .GroupBy(rec =>
                {
                    if (rec.Contains(ownerAttributeName) && rec[ownerAttributeName] is EntityReference ownerRef)
                    {
                        return ownerRef.Id;
                    }
                    else
                    {
                        log.ERROR($"Record does not contain a valid {ownerAttributeName} attribute or it is not of type EntityReference.");
                        return Guid.Empty;
                    }
                })
                .Where(g => g.Key != Guid.Empty)
                .ToDictionary(g => g.Key, g => g.Count());

                if (!userLoadCounts.Any())
                {
                    log.WARNING("No users found with the specified conditions.");
                    return new Entity();
                }

                // Вычисляем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = userLoadCounts
                    .OrderBy(entry => entry.Value)
                    .FirstOrDefault().Key;

                log.INFO($"Less loaded user ID: {leastLoadedUserId}");

                return new Entity(User.EntityLogicalName, leastLoadedUserId); // Возвращаем пользователя с наименьшей нагрузкой
            }
            catch (Exception ex)
            {
                log.ERROR($"Error in {nameof(GetLeastLoadedEntity)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(GetLeastLoadedEntity)} method of PluginHelper.", ex);
            }
        }

        /// <summary>
        /// Устанавливает условия для фильтрации записей на основе списка идентификаторов пользователей и статусов.
        /// </summary>
        /// <param name="components">Массив кортежей, содержащих имя столбца, оператор условия и значение.</param>
        /// <returns>Список условий для фильтрации записей.</returns>
        public static List<ConditionExpression> SetConditionsExpressions(params (string columnName, ConditionOperator conditionOperator, object value)[] components)
        {
            var conditions = new List<ConditionExpression>();
            foreach (var component in components)
            {
                if (component.value is Guid[] guidArray)
                {
                    conditions.Add(new ConditionExpression(component.columnName, component.conditionOperator, guidArray.Cast<object>().ToArray()));
                }
                else
                {
                    conditions.Add(new ConditionExpression(component.columnName, component.conditionOperator, component.value));
                }
            }

            return conditions;
        }

        /// <summary>
        /// Проверяет корректность EntityReference.
        /// </summary>
        /// <param name="entityReference">EntityReference для проверки.</param>
        /// <param name="pluginName">Имя плагина.</param>
        /// <param name="parameterName">Имя параметра.</param>
        /// <param name="logger">Экземпляр Logger для логирования.</param>
        /// <returns>Проверенный EntityReference.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если EntityReference некорректен.</exception>
        public static EntityReference ValidateEntityReference(EntityReference entityReference, string pluginName, string parameterName, Logger logger)
        {
            Logger log = logger;
            try
            {
                if (entityReference == null)
                {
                    log.ERROR($" Exception in plugin {pluginName} in {parameterName} EntityReference is Null!");
                    throw new ArgumentNullException($" Exception in plugin {pluginName} in {parameterName} EntityReference is Null!");
                }

                if (entityReference.Id == Guid.Empty)
                {
                    log.ERROR($" Exception in plugin {pluginName} in {parameterName} entityReference.Id is Empty!");
                    throw new ArgumentNullException($" Exception in plugin {pluginName} in {parameterName} entityReference.Id is Empty!");
                }

                if (string.IsNullOrWhiteSpace(entityReference.LogicalName))
                {
                    log.ERROR($" Exception in plugin {pluginName} in {parameterName} entityReference.LogicalName is Null or Empty!");
                    throw new ArgumentNullException($" Exception in plugin {pluginName} in {parameterName} entityReference.LogicalName is Null or Empty!");
                }

                return entityReference;
            }
            catch (Exception ex)
            {
                log.ERROR($"Exception in plugin {pluginName} in {parameterName}: {ex.Message}");
                throw new Exception($"Exception in plugin {pluginName} in {parameterName}", ex);
            }
        }

        /// <summary>
        /// Проверяет корректность нескольких EntityReference с использованием кортежей в параллельном режиме.
        /// </summary>
        /// <param name="logger">Экземпляр Logger для логирования.</param>
        /// <param name="entityReferencesAttributesInfo">Массив кортежей, содержащих EntityReference, имя плагина и имя параметра.</param>
        /// <exception cref="AggregateException">Выбрасывается, если одна или несколько проверок завершились с ошибкой.</exception>
        public static void ValidateEntityReferencesWithTuples(Logger logger, params (EntityReference entityReference, string pluginName, string parameterName)[] entityReferencesAttributesInfo)
        {
            var exceptions = new ConcurrentBag<Exception>();
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.ForEach(entityReferencesAttributesInfo, parallelOptions, attribute =>
            {
                try
                {
                    ValidateEntityReference(attribute.entityReference, attribute.pluginName, attribute.parameterName, logger);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });

            if (exceptions.Any())
            {
                throw new AggregateException("Exceptions in ValidateEntityReferencesWithTuples", exceptions);
            }
        }

        /// <summary>
        /// Создает ColumnSet на основе списка имен атрибутов.
        /// </summary>
        /// <param name="attributeNames">Список имен атрибутов.</param>
        /// <returns>ColumnSet, содержащий указанные атрибуты.</returns>
        public static ColumnSet CreateColumnSet(params string[] attributeNames)
        {
            return new ColumnSet(attributeNames);
        }
    }
}
