using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;
using AwaraIT.Training.Domain.Extensions;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    public class PossibleDealLessBusyUserAssignmentPlugin
    {
        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {

                    var clietntD365 = (IOrganizationService)client;

                    var tsPl = new TestPlugin(clietntD365);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");

            }
        }
    }

    internal class TestPlugin
    {
        private readonly IOrganizationService _service;

        private Logger _log;
        public TestPlugin(IOrganizationService service)
        {

            _service = service;
            Execute(_service);



        }


        public void Execute(IOrganizationService wrapper)
        {
            _log = new Logger(wrapper);
            ColumnSet allColumns = new ColumnSet(true);
            var res = wrapper.Retrieve(PossibleDeal.EntityLogicalName, new Guid("ffd76283-4cad-ef11-b8e9-000d3a5c09a6"), allColumns);

            var TargetEntity = res.ToEntity<PossibleDeal>();
                     

            if (TargetEntity == null || TargetEntity.LogicalName != PossibleDeal.EntityLogicalName)
                return;

            var possibleDeal = TargetEntity;

            if (!possibleDeal.Attributes.Contains(PossibleDeal.Metadata.TerritoryReference))
                return;

            Guid territoryId = possibleDeal.TerritoryReference.Id;

            QueryExpression query = new QueryExpression("systemuser") // Сущность 'systemuser' - таблица пользователей системы
            {
                // Указываем, какие поля хотим получить: ID пользователя 
                ColumnSet = new ColumnSet("systemuserid"),
                LinkEntities =
                {
                 // Связываем 'systemuser' с 'teammembership' для получения информации о членстве пользователя в командах
                  new LinkEntity
                  {
                     LinkFromEntityName = "systemuser",
                     LinkFromAttributeName = "systemuserid",
                     LinkToEntityName = "teammembership",
                     LinkToAttributeName = "systemuserid",
                     LinkEntities =
                     {
                      // Дополнительно связываем 'teammembership' с сущностью 'team' для фильтрации по территории
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
                              // Условие: ищем команды, которые связаны с определённой территорией
                               new ConditionExpression("territoryid", ConditionOperator.Equal, territoryId)
                             }
                          }
                       }
                     }
                  }
                }
            };


            // Находим всех пользователей, которые состоят в рабочей группе, связанной с территорией
            /* QueryExpression query = new QueryExpression(User.EntityLogicalName) // Сущность 'systemuser' - таблица пользователей системы
             {
                 // Указываем, какие поля хотим получить: ID пользователя и его полное имя
                 ColumnSet = new ColumnSet(User.Metadata.SystemUserId), // Поля: 'systemuserid' — уникальный идентификатор пользователя
                 LinkEntities =
                     {
                         new LinkEntity
                         {
                             LinkFromEntityName = User.EntityLogicalName,
                             LinkFromAttributeName = User.Metadata.SystemUserId,         // Поле ID пользователя в сущности 'systemuser'
                             LinkToEntityName = Teammembership.EntityLogicalName,        // Сущность 'teammembership' — таблица членства пользователей в командах
                             LinkToAttributeName = Teammembership.Metadata.SystemUserId, // Поле ID пользователя в сущности 'teammembership', связывающее пользователей с их членством
                             LinkEntities =
                             {
                                 new LinkEntity
                                 {
                                     LinkFromEntityName = Teammembership.EntityLogicalName,  // Поле ID команды в сущности 'teammembership'
                                     LinkFromAttributeName = Teammembership.Metadata.TeamId, // Сущность 'team' — таблица команд
                                     LinkToEntityName = WorkGroup.EntityLogicalName,          // Поле ID команды в сущности 'team', связывающее команды с их членами
                                     LinkToAttributeName = WorkGroup.Metadata.TeamId,
                                     LinkCriteria =
                                     {
                                         Conditions =
                                         {
                                             // Условие: ищем команды, которые связаны с определённой территорией
                                             // Условие фильтрации по 'territoryid' может относится к сущности 'territory', фильтрующей по идентификатору территории
                                             new ConditionExpression( "territoryid", ConditionOperator.Equal, territoryId) 
                                         }
                                     }
                                 }
                             }
                         }
                     }
             };*/

            EntityCollection users = wrapper.RetrieveMultiple(query);

            if (users.Entities.Count == 0)
                return;

            // Ищем самого ненагруженного пользователя
            Entity leastLoadedUser = null;
            int minLoad = int.MaxValue;

            foreach (Entity user in users.Entities)
            {
                QueryExpression loadQuery = new QueryExpression(PossibleDeal.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PossibleDeal.Metadata.Id),
                    Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("ownerid", ConditionOperator.Equal, user.Id),
                                new ConditionExpression("statuscode", ConditionOperator.Equal, PossibleDealStatusEnums.InWork.ToIntValue()) // "В работе"
                            }
                        }
                };

                int load = wrapper.RetrieveMultiple(loadQuery).Entities.Count;

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
                wrapper.Update(possibleDeal);
            }
        }
    }
}



