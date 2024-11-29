using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PosibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.Collections.Generic;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Kuralbek.Plugins.Hellpers;
using AwaraIT.Kuralbek.Plugins.Helpers;
using AwaraIT.Training.Domain.Models.Crm;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    public class PossibleDealLessBusyUserAssignmentPlugin : PluginBase
    {
        private readonly string _teamName = "fnt_Менеджеры по продажам";
        private Logger _log;

        public PossibleDealLessBusyUserAssignmentPlugin() : base()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(PosibleDeal.EntityLogicalName)
                .When(PluginStage.PreOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            try
            {
                var posibleDeal = wrapper?.TargetEntity.ToEntity<PosibleDeal>();
                var territoryId = posibleDeal.TerritoryReference.Id;
                // Получаем всех пользователей из рабочих групп, связанных с территорией
                List<Guid> usersIdList = GetUsersByTerritoryId(wrapper, territoryId);

                // Условия для поиска записей 
                var conditionsExpressions = PluginHelper.SetConditionsExpressions(usersIdList, PosibleDealStepStatus.InProgress.ToIntValue());
                // Получаем наименее загруженного пользователя 
                var responsibleUser = PluginHelper.GetLeastLoadedEntity(wrapper, conditionsExpressions, PosibleDeal.EntityLogicalName);

                if (responsibleUser.Id == Guid.Empty)
                {
                    return;
                }

                posibleDeal.OwnerId = responsibleUser.ToEntityReference();
            }
            catch (Exception ex)
            {
                _log.ERROR(ex, "Error in AssignLeastBusyUser");
                throw;
            }
        }

        private List<Guid> GetUsersByTerritoryId(IContextWrapper context, Guid territoryId)
        {
            try
            {
                // Запрос для полчения пользоввателей, связанных с территорией
                var userQuery = new QueryExpression(User.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(User.Metadata.SystemUserId),
                    LinkEntities =
                    {
                        new LinkEntity(User.EntityLogicalName, Teammembership.EntityLogicalName, User.Metadata.SystemUserId, Teammembership.Metadata.SystemUserId, JoinOperator.Inner)
                        {
                            LinkEntities =
                            {
                                new LinkEntity(Teammembership.EntityLogicalName, "fnt_territory_team", "teamid", "teamid", JoinOperator.Inner)
                                {
                                    LinkCriteria = new FilterExpression
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("fnt_territoryid", ConditionOperator.Equal, territoryId)
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                // Execute the query and get the list of users
                var userEntities = context.Service.RetrieveMultiple(userQuery).Entities;

                var userIds = userEntities.Select(u => u.GetAttributeValue<Guid>(User.Metadata.SystemUserId)).ToList();

                return userIds;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(GetUsersByTerritoryId)}: {ex.Message}, {ex}");
                throw new Exception($"Error in {nameof(GetUsersByTerritoryId)}: {ex.Message}", ex);
            }
        }
    }
}

