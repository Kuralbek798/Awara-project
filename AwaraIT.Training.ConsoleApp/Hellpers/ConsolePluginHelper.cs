using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System;
using AwaraIT.Training.Application.Core;
using System.Linq;
using AwaraIT.Training.Domain.Models.Crm;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using System.Windows;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Helpers
{
    public static class ConsolePluginHelper
    {
        /// <summary>
        /// Получает сущность с наименьшей нагрузкой на основе заданных условий.
        /// </summary>
        /// <param name="entityRecords">Коллекция сущностей для анализа.</param>
        /// <param name="ownerAttributeName">Имя атрибута владельца в сущности.</param>
        /// <param name="logger">Экземпляр Logger для логирования.</param>
        /// <returns>Сущность с наименьшей нагрузкой.</returns>
        /// <exception cref="InvalidPluginExecutionException">Выбрасывается при возникновении ошибки во время выполнения запроса.</exception>
        public static Entity GetLeastLoadedEntity(DataCollection<Entity> entityRecords, string ownerAttributeName, Logger logger)
        {
            Logger log = logger;
            try
            {
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
        /// Создает ColumnSet на основе списка имен атрибутов.
        /// </summary>
        /// <param name="getAll">Флаг для выбора всех столбцов.</param>
        /// <param name="attributeNames">Список имен атрибутов.</param>
        /// <returns>ColumnSet, содержащий указанные атрибуты.</returns>
        public static ColumnSet CreateColumnSet(bool getAll = false, params string[] attributeNames)
        {
            if (getAll)
            {
                return new ColumnSet(true);
            }
            else
            {
                return new ColumnSet(attributeNames);
            }
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
    }
}













/*using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using AwaraIT.Training.Domain.Extensions;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;

namespace AwaraIT.Kuralbek.Plugins.Hellpers
{
    public static class PluginHelper
    {
        private static Logger _log;
        public static Entity GetLeastLoadedUser(IContextWrapper wrapper, List<Guid> usersId)
        {
            try
            {
                var loadQuery = new QueryExpression(Interest.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(EntityCommon.OwnerId),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                            new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, usersId.ToArray()),
                            new ConditionExpression(Interest.Metadata.Status, ConditionOperator.Equal, InterestStepStatus.InProgress.ToIntValue()),
                            new ConditionExpression(Interest.Metadata.Status, ConditionOperator.Equal, InterestStepStatus.New.ToIntValue()),
                        }
                    },

                };

                // Получаем записи интересов
                var interestRecords = wrapper.Service.RetrieveMultiple(loadQuery).Entities;//.Select(e => e.ToEntity<Interest>());

                // Подсчитываем интересы для каждого пользователя
                var userLoadCounts = interestRecords
                  .GroupBy(rec => rec.ToEntity<Interest>().OwnerId.ProductCartId)
                  .ToDictionary(g => g.Key, g => g.Count());

                //Получаем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = userLoadCounts
                  .OrderBy(entry => entry.Value)
                  .FirstOrDefault().Key;

                //  _log.INFO($"{_teamName} {DataForLogs.GetDataStringFromDictionary(userLoadCounts)}");
                _log.INFO($"Less loaded user ID:{leastLoadedUserId}");

                return new Entity(User.EntityLogicalName, leastLoadedUserId);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(GetLeastLoadedUser)} {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(GetLeastLoadedUser)}: {ex.Message}", ex);
            }
        }
    }
}
*/