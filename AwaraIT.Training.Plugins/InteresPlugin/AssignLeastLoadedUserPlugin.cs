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

        private void Execute(IContextWrapper context)
        {
            _log = new Logger(context.Service);
            var posibleDeal = context?.TargetEntity.ToEntity<PosibleDeal>();

            try
            {
                // Получаем всех пользователей из рабочих групп, связанных с территорией
                List<Entity> users = GetAvailableUsers(context.Service, posibleDeal);

                // Определяем наименее загруженного пользователя
               // Entity leastLoadedUser = FindLeastLoadedUser(service, users);

                //if (leastLoadedUser != null)
                //{
                //    // Назначаем ответственного за сделку
                //    possibleDeal["responsibleuser"] = leastLoadedUser.ToEntityReference();
                //}
            }
            catch (Exception ex)
            {

                _log.ERROR(ex, "Error in AssignLeastBusyUser");
                throw;
            }

        }


        private List<Entity> GetAvailableUsers(IOrganizationService service, PosibleDeal posibleDeal)
        {
            var territoryId = posibleDeal.TerritoryReference.Id;



            var query = new QueryExpression(WorkGroup.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(/*WorkGroup.Metadata.TeamId*/ true),
                Criteria = new FilterExpression
                {    
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                            {
                                new ConditionExpression("territory", ConditionOperator.Equal, territoryId)
                            }
                }
            };

            /*    var workingGroupEntities = service.RetrieveMultiple(query).Entities;
                HashSet<Guid> userIds = new HashSet<Guid>();

                foreach (var group in workingGroupEntities)
                {
                    var usersQuery = new QueryExpression("systemuser")
                    {
                        ColumnSet = new ColumnSet("systemuserid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                                    {
                                        new ConditionExpression("workinggroupid", ConditionOperator.Equal, group.Id)
                                    }
                        }
                    };

                    var userEntities = service.RetrieveMultiple(usersQuery).Entities;
                    foreach (var user in userEntities)
                    {
                        userIds.Add(user.Id);
                    }
                }

                return userIds.Select(id => service.Retrieve("systemuser", id, new ColumnSet("systemuserid"))).ToList();*/

            return null;
        }

        /*private Entity FindLeastLoadedUser(IOrganizationService service, List<Entity> users)
        {
            Dictionary<Entity, int> userLoad = new Dictionary<Entity, int>();

            foreach (var user in users)
            {
                var dealsQuery = new QueryExpression("possiblesdeal")
                {
                    ColumnSet = new ColumnSet("status"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                                {
                                    new ConditionExpression("responsibleuser", ConditionOperator.Equal, user.Id),
                                    new ConditionExpression("status", ConditionOperator.Equal, "В работе")
                                }
                    }
                };

                int inProgressCount = service.RetrieveMultiple(dealsQuery).Entities.Count;
                userLoad[user] = inProgressCount;
            }

            return userLoad.OrderBy(ul => ul.Value).FirstOrDefault().Key;
        }*/


    }
}

