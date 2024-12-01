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


namespace AwaraIT.Kuralbek.Plugins.Helpers
{
    public static class PluginHelper
    {
        private static Logger _log;

        /// <summary>
        /// Получает сущность с наименьшей нагрузкой на основе заданных условий.
        /// </summary>
        /// <param name="wrapper">Экземпляр IOrganizationService для выполнения запроса.</param>
        /// <param name="conditionExpressions">Список условий для фильтрации записей.</param>
        /// <param name="entityLogicalName">Логическое имя сущности для запроса.</param>
        /// <param name="ownerAttributeName">Имя атрибута владельца в сущности.</param>
        /// <returns>Сущность с наименьшей нагрузкой.</returns>
        /// <exception cref="InvalidPluginExecutionException">Выбрасывается при возникновении ошибки во время выполнения запроса.</exception>
        public static Entity GetLeastLoadedEntity(
            IContextWrapper wrapper,
            List<ConditionExpression> conditionExpressions,
            string entityLogicalName,
            string ownerAttributeName,
            Logger log)
        {
            try
            {
                _log = log;

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
                    _log.WARNING("No records found matching the specified conditions.");
                    return null;
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
                        _log.ERROR($"Record does not contain a valid {ownerAttributeName} attribute or it is not of type EntityReference.");
                        return Guid.Empty;
                    }
                })
                    .Where(g => g.Key != Guid.Empty)
                    .ToDictionary(g => g.Key, g => g.Count());
                if (!userLoadCounts.Any())
                {
                    _log.WARNING("No users found with the specified conditions.");
                    return null;
                }

                // Вычисляем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = userLoadCounts
                    .OrderBy(entry => entry.Value)
                    .FirstOrDefault().Key;

                _log.INFO($"Less loaded user ID: {leastLoadedUserId}");

                return new Entity(User.EntityLogicalName, leastLoadedUserId); // Возвращаем пользователя с наименьшей нагрузкой

            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in {nameof(GetLeastLoadedEntity)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(GetLeastLoadedEntity)} method of PluginHelper.", ex);
            }
        }

        /// <summary>
        /// Устанавливает условия для фильтрации записей на основе списка идентификаторов пользователей и статусов.
        /// </summary>
        /// <param name="usersIdList">Список идентификаторов пользователей.</param>
        /// <param name="statusAttributeName">Имя атрибута статуса в сущности.</param>
        /// <param name="stepStatus1">Первый статус для фильтрации.</param>
        /// <param name="stepStatus2">Второй статус для фильтрации (необязательный).</param>
        /// <returns>Список условий для фильтрации записей.</returns>
        public static List<ConditionExpression> SetConditionsExpressions(List<Guid> usersIdList, string statusAttributeName, int stepStatus1, int stepStatus2 = 0)
        {


            var conditions = new List<ConditionExpression>
               {
               new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, usersIdList.ToArray()),
               new ConditionExpression(statusAttributeName, ConditionOperator.Equal, stepStatus1)
                };
            return conditions;

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