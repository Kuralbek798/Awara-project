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

        public static Entity GetLeastLoadedEntity(IContextWrapper wrapper, List<ConditionExpression> conditionExpressions, string entityLogicalName)
        {
            try
            {
                _log = new Logger(wrapper.Service);

                // Создаем запрос
                var loadQuery = new QueryExpression(entityLogicalName)
                {
                    ColumnSet = new ColumnSet(EntityCommon.OwnerId),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And
                    }
                };

                // Добавляем условия в запрос
                conditionExpressions.ForEach(condition => loadQuery.Criteria.AddCondition(condition));

                // Делаем запрос
                var entityRecords = wrapper.Service.RetrieveMultiple(loadQuery).Entities;

                // Считаем количество запсей для каждого пользователя
                var userLoadCounts = entityRecords
                    .GroupBy(rec => (Guid)rec[EntityCommon.OwnerId])
                    .ToDictionary(g => g.Key, g => g.Count());

                // Вычисляем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = userLoadCounts
                    .OrderBy(entry => entry.Value)
                    .FirstOrDefault().Key;

                _log.INFO($"Less loaded user ID: {leastLoadedUserId}");

                return new Entity(User.Metadata.SystemUserId, leastLoadedUserId); // Возвращаем пользователя с наименьшей нагрукой

            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in {nameof(GetLeastLoadedEntity)}: {ex.Message}, {ex}");
                throw new Exception($"Error in {nameof(GetLeastLoadedEntity)}: {ex.Message}", ex);
            }
        }

        public static List<ConditionExpression> SetConditionsExpressions(List<Guid> usersIdList, int stepStatus1, int stepStatus2 = 0)
        {
            var conditions = new List<ConditionExpression>
            {
               new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, usersIdList.ToArray()),
               new ConditionExpression(Interest.Metadata.Status, ConditionOperator.Equal, stepStatus1)
            };

            if (stepStatus2 != 0)
            {
                conditions.Add(new ConditionExpression(Interest.Metadata.Status, ConditionOperator.Equal, stepStatus2));
            }

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
                  .GroupBy(rec => rec.ToEntity<Interest>().OwnerId.Id)
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