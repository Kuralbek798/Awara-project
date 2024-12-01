using AwaraIT.Kuralbek.Plugins.Helpers;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Extensions;
using AwaraIT.Training.Domain.Models.Crm;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PosibleDeal;

namespace AwaraIT.Kuralbek.Plugins.Actions
{

    public static class PossibleDealLessBusyUserAssignmentPluginTest
    {

        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {

                    var clietntD365 = (IOrganizationService)client;

                    var tsPl = new Test1(clietntD365);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");

            }
        }
    }

    public class Test1
    {
        private readonly string _teamName = "fnt_Менеджер по продажам Казахстан";
        private Logger _log;
        private IOrganizationService _service;

        public Test1(IOrganizationService service)
        {
            _service = service;
            Execute(_service);

        }

        /// <summary>
        /// Основной метод выполнения плагина, который назначает владельца возможной сделки наименее загруженному пользователю.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void Execute(IOrganizationService wrapper)
        {
            _log = new Logger(wrapper);

            try
            {

                //var posibleDeal = wrapper.?.TargetEntity.ToEntity<PosibleDeal>();
                //var territoryId = posibleDeal.TerritoryReference.ProductCartId;

                var posibleDeal = new PosibleDeal() { TerritoryReference = new EntityReference { Id = Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6") } };
                var territoryId = posibleDeal.TerritoryReference.Id;
                // Получаем всех пользователей из рабочих групп, связанных с территорией
                List<Guid> usersIdList = GetUsersByTerritoryId(wrapper, territoryId);

                // Условия для поиска записей 
                var conditionsExpressions = PluginHelper.SetConditionsExpressions(usersIdList, PosibleDeal.Metadata.Status, PosibleDealStepStatus.InProgress.ToIntValue());
                // Получаем наименее загруженного пользователя 
                var responsibleUser = PluginHelper.GetLeastLoadedEntity(wrapper, conditionsExpressions, PosibleDeal.EntityLogicalName, EntityCommon.OwnerId, _log);


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
