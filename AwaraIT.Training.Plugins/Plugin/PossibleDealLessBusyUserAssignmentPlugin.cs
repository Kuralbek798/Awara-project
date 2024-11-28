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

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    public class PossibleDealLessBusyUserAssignmentPlugin : PluginBase
    {
        private readonly string _teamName = "fnt_Менеджеры по продажам";
        private readonly Logger _log;

        public PossibleDealLessBusyUserAssignmentPlugin() : base()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(PossibleDeal.EntityLogicalName)
                .When(PluginStage.PreOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper context)
        {
           // var service = context.Service;
            var preImage = context?.PreImage.ToEntity<PossibleDeal>();
            var targetEntity = context?.TargetEntity.ToEntity<PossibleDeal>();
            var postImage = context?.PostImage.ToEntity<PossibleDeal>();


            if(targetEntity == null && preImage == null && postImage == null)
            {
                _log.ERROR("preImage possibleDeal postImage are null");
                return;
            }


            if(preImage.TerritoryReference.Id != null)
            {
                _log.INFO($"preImage.TerritoryReference {preImage.TerritoryReference.Id} {preImage.TerritoryReference.LogicalName}");

            }
            else
            {
                _log.WARNING($"preImage.TerritoryReference is null");
            } 


            if(targetEntity.TerritoryReference.Id != null)
            {
             
                _log.INFO($"targetEntity.TerritoryReference {targetEntity.TerritoryReference.Id} {targetEntity.TerritoryReference.LogicalName}");

            }
            else
            {
                _log.WARNING($"targetEntity.TerritoryReference is null");
            }
            
            if(preImage.TerritoryReference.Id != null)
            {
               
                _log.INFO($"postImage.TerritoryReference {postImage.TerritoryReference.Id} {postImage.TerritoryReference.LogicalName}");
            }
            else
            {
                _log.WARNING($"postImage.TerritoryReference is null");
            }
            /*
                        try
                        {
                            // Получаем всех пользователей из рабочих групп, связанных с территорией
                            List<Entity> users = GetAvailableUsers(service, territoryRef.Id);

                            // Определяем наименее загруженного пользователя
                            Entity leastLoadedUser = FindLeastLoadedUser(service, users);

                            if (leastLoadedUser != null)
                            {
                                // Назначаем ответственного за сделку
                                possibleDeal["responsibleuser"] = leastLoadedUser.ToEntityReference();
                            }
                        }
                        catch (Exception ex)
                        {

                            _log.ERROR(ex, "Error in AssignLeastBusyUser");
                            throw;
                        }*/

        }
/*
        private List<Entity> GetAvailableUsers(IOrganizationService service, Guid territoryId)
        {
            var query = new QueryExpression("workinggroup")
            {
                ColumnSet = new ColumnSet("workinggroupid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("territory", ConditionOperator.Equal, territoryId)
                    }
                }
            };

            var workingGroupEntities = service.RetrieveMultiple(query).Entities;
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

            return userIds.Select(id => service.Retrieve("systemuser", id, new ColumnSet("systemuserid"))).ToList();
        }

        private Entity FindLeastLoadedUser(IOrganizationService service, List<Entity> users)
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
