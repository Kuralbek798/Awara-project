using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.Collections.Generic;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Kuralbek.Plugins.Hellpers;
using AwaraIT.Kuralbek.Plugins.Helpers;
using AwaraIT.Training.Domain.Models.Crm;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Плагин для назначения владельца возможной сделки наименее загруженному пользователю.
    /// </summary>
    public class PossibleDealLessBusyUserAssignmentPlugin : PluginBase
    {
        private readonly string _teamName = "fnt_Менеджер по продажам Казахстан";
        private Logger _log;

        public PossibleDealLessBusyUserAssignmentPlugin() : base()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(PossibleDeal.EntityLogicalName)
                .When(PluginStage.PreOperation)
                .Execute(Execute);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который назначает владельца возможной сделки наименее загруженному пользователю.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void Execute(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            try
            {
                var posibleDeal = wrapper?.TargetEntity.ToEntity<PossibleDeal>();
                var territoryId = posibleDeal.TerritoryReference.Id;
                // Получаем всех пользователей из рабочих групп, связанных с территорией
                List<Guid> usersIdList = GetUsersByTerritoryId(wrapper.Service, territoryId);

                // Условия для поиска записей 
                var conditionsExpressions = PluginHelper.SetConditionsExpressions(usersIdList, PossibleDeal.Metadata.Status, PossibleDealStepStatus.InProgress.ToIntValue());
                // Получаем наименее загруженного пользователя 
                var responsibleUser = PluginHelper.GetLeastLoadedEntity(wrapper, conditionsExpressions, PossibleDeal.EntityLogicalName, EntityCommon.OwnerId, _log);


                if (responsibleUser is Entity && responsibleUser.Id == Guid.Empty)
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

        /// <summary>
        /// Получает список идентификаторов пользователей, связанных с указанной территорией.
        /// </summary>
        /// <param name="context">Контекст выполнения плагина.</param>
        /// <param name="territoryId">Идентификатор территории.</param>
        /// <returns>Список GUID, представляющих идентификаторы пользователей, связанных с указанной территорией.</returns>
        /// <exception cref="Exception">Выбрасывается, когда происходит ошибка во время выполнения запроса.</exception>
        private List<Guid> GetUsersByTerritoryId(IOrganizationService context, Guid territoryId)
        {
            try
            {
                // Запрос для получения пользователей, связанных с территорией
                var userQuery = new QueryExpression(User.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(User.Metadata.SystemUserId),
                    LinkEntities =
                    {
                        new LinkEntity(User.EntityLogicalName, TeammembershipNN.EntityLogicalName, User.Metadata.SystemUserId, TeammembershipNN.Metadata.SystemUserId, JoinOperator.Inner)
                        {
                            LinkEntities =
                            {
                                new LinkEntity(TeammembershipNN.EntityLogicalName, TerritoryTeamNN.EntityLogicalName, TeammembershipNN.Metadata.TeamId, TerritoryTeamNN.Metadata.TeamId, JoinOperator.Inner)
                                {
                                    LinkCriteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions =
                                        {
                                            new ConditionExpression(TerritoryTeamNN.Metadata.TerritoryId, ConditionOperator.Equal, territoryId),
                                        }
                                    },
                                    LinkEntities =
                                    {
                                        new LinkEntity(Team.EntityLogicalName, Team.EntityLogicalName, Team.Metadata.TeamId, Team.Metadata.TeamId, JoinOperator.Inner)
                                        {
                                            LinkCriteria = new FilterExpression
                                            {
                                                FilterOperator = LogicalOperator.And,
                                                Conditions =
                                                {
                                                    new ConditionExpression(Team.Metadata.Name, ConditionOperator.Equal, _teamName)
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                // Выполняем запрос и получаем список пользователей
                var userEntities = context.RetrieveMultiple(userQuery).Entities;

                var userIds = userEntities.Select(u => u.GetAttributeValue<Guid>(User.Metadata.SystemUserId)).ToList();

                return userIds;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(GetUsersByTerritoryId)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(GetUsersByTerritoryId)} method.", ex);
            }
        }
    }
}





