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

namespace AwaraIT.Training.Plugins.InteresPlugin
{
    public class AssignLeastLoadedUserPlugin : PluginBase
    {
        private Logger _log;

        public AssignLeastLoadedUserPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(PosibleDeal.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        public void Execute(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            if (wrapper.TargetEntity == null || wrapper.TargetEntity.LogicalName != PosibleDeal.EntityLogicalName)
                return;

            var possibleDeal = wrapper.TargetEntity;

            if (!possibleDeal.Attributes.Contains(PosibleDeal.Metadata.TerritoryReference))
                return;

            Guid territoryId = ((EntityReference)possibleDeal[PosibleDeal.Metadata.TerritoryReference]).Id;

            // Находим всех пользователей, которые состоят в рабочей группе, связанной с территорией
            QueryExpression query = new QueryExpression("systemuser")
            {
                ColumnSet = new ColumnSet("systemuserid", "fullname"),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "systemuser",
                        LinkFromAttributeName = "systemuserid",
                        LinkToEntityName = "teammembership",
                        LinkToAttributeName = "systemuserid",
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = "teammembership",
                                LinkFromAttributeName = "teamid",
                                LinkToEntityName = "team",
                                LinkToAttributeName = "teamid",
                                LinkCriteria =
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("territoryid", ConditionOperator.Equal, territoryId)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            EntityCollection users = wrapper.Service.RetrieveMultiple(query);

            if (users.Entities.Count == 0)
                return;

            // Ищем самого ненагруженного пользователя
            Entity leastLoadedUser = null;
            int minLoad = int.MaxValue;

            foreach (Entity user in users.Entities)
            {
                QueryExpression loadQuery = new QueryExpression(PosibleDeal.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet("posibledealid"),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("ownerid", ConditionOperator.Equal, user.Id),
                            new ConditionExpression("statuscode", ConditionOperator.Equal, PosibleDealStatusEnums.InWork.ToIntValue()) // "В работе"
                        }
                    }
                };

                int load = wrapper.Service.RetrieveMultiple(loadQuery).Entities.Count;

                if (load < minLoad)
                {
                    minLoad = load;
                    leastLoadedUser = user;
                }
            }

            if (leastLoadedUser != null)
            {
                // Назначаем самого ненагруженного пользователя ответственным
                possibleDeal["ownerid"] = new EntityReference("systemuser", leastLoadedUser.Id);
                wrapper.Service.Update(possibleDeal);
            }
        }
    }
}
