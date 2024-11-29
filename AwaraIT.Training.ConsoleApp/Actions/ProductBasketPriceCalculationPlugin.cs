
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PosibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.Collections.Generic;
using AwaraIT.Kuralbek.Plugins.Actions;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;

namespace AwaraIT.Kuralbek.Plugins

{
    public class ProductBasketPriceCalculationPlugin
    {
        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {

                    var clietntD365 = (IOrganizationService)client;

                    var tsPl = new Test(clietntD365);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");

            }
        }

    }

    internal class Test
    {
        private Logger _log;
        private readonly IOrganizationService _service;

        private readonly string _teamName = "fnt_Менеджеры по продажам";
        public Test(IOrganizationService service)
        {
            _service = service;
            Execute();
        }

        public void Execute()
        {
            _log = new Logger(_service);



            var interestConditions = new Dictionary<string, object>
                         {
                            { Interest.Metadata.Status, InterestStepStatus.InProgress.ToIntValue() },
                            { Interest.Metadata.Status, InterestStepStatus.New.ToIntValue() }
                         };

            GetUsersByTerritoryId(Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));




            //GetTeamsByTerritory(Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));
            // GetTestUsersWithTerritory(_service);

        }


        private List<Guid> GetUsersByTerritoryId(Guid territoryId)
        {




            try
            {
                // Query to get users associated with the specified territory
                var userQuery = new QueryExpression(User.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(User.Metadata.SystemUserId), // Field we want to retrieve
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
                var userEntities = _service.RetrieveMultiple(userQuery).Entities;

                var userIds = userEntities.Select(u => u.GetAttributeValue<Guid>(User.Metadata.SystemUserId)).ToList();

                return userIds;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(GetUsersByTerritoryId)}: {ex.Message}, {ex}");
                throw new Exception($"Error in {nameof(GetUsersByTerritoryId)}: {ex.Message}", ex);
            }
        }



        private WorkGroup GetTeamsByTerritory(Guid territoryId)
        {
            var teamIds = new List<Guid>();

            // Запрос для получения команд, связанных с заданной территорией
            var teamQuery = new QueryExpression(User.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("teamid", "name"),
                LinkEntities =
                {
                    new LinkEntity("team", "fnt_territory_team", "teamid", "teamid", JoinOperator.Inner)
                    {
                        LinkCriteria = new FilterExpression
                        {
                            Conditions =
                            {
                                // Фильтрация по ID территории
                                new ConditionExpression("fnt_territoryid", ConditionOperator.Equal, territoryId)
                            }
                        }
                    }
                }
            };

            // Выполняем запрос для получения команд
            var teamEntitY = _service.RetrieveMultiple(teamQuery).Entities.ToList().Select(g => g.ToEntity<WorkGroup>()).FirstOrDefault();


            Console.WriteLine($"{teamEntitY.Name.ToString()}, {teamEntitY.TeamId.ToString()}, {teamEntitY.LogicalName.ToString()}");
            // Поскольку вы хотите получить сущности команд, просто вернем их
            return teamEntitY;
        }














        private List<Entity> GetTestUsersWithTerritory(IOrganizationService service)
        {
            // Создаем запрос для сущности fnt_test_users
            var query = new QueryExpression("fnt_territory_team")
            {
                ColumnSet = new ColumnSet(true)
            };

            // Выполняем запрос и получаем результаты
            var results = service.RetrieveMultiple(query).Entities.ToList();

            return results; // Возвращаем список записей
        }




    }
}


