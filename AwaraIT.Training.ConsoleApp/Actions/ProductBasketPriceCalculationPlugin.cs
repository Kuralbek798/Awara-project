
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

            //   GetTeamIdsByTerritory(Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));
            // GetTeamIdsByTerritory(Guid.Parse("c294544e-80a6-ef11-8a6a-000d3a5c09a6"));
            GetTerritoriesForTeam(Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));
            // RetrieveTerritoryTeamData();
            // GetTestUsersWithTerritory(_service);

        }


        private void RetrieveTerritoryTeamData()
        {
            // Создаем запрос для получения данных из fnt_territory_team
            var query = new QueryExpression("fnt_territory_team")
            {
                ColumnSet = new ColumnSet(true) // Получаем все колонки
            };

            // Выполняем запрос
            var entities = _service.RetrieveMultiple(query).Entities;

            // Выводим данные на консоль
            foreach (var entity in entities)
            {
                Console.WriteLine("fnt_territory_team Record:");
                foreach (var attribute in entity.Attributes)
                {
                    Console.WriteLine($"{attribute.Key}: {attribute.Value}");
                }
                Console.WriteLine();
            }
        }


        private List<Entity> GetTestUsersWithTerritory(IOrganizationService service)
        {
            // Создаем запрос для сущности fnt_test_users
            var query = new QueryExpression("fnt_test_users_fnt_territory")
            {
                ColumnSet = new ColumnSet(true)
            };

            // Выполняем запрос и получаем результаты
            var results = service.RetrieveMultiple(query).Entities.ToList();

            return results; // Возвращаем список записей
        }

        private List<Entity> GetTerritoriesForTeam(Guid teamId)
        {
            // Создаем запрос для связующей таблицы fnt_territory_team
            var query = new QueryExpression("fnt_territory_team") // Имя связующей сущности
            {
                ColumnSet = new ColumnSet(true), // Выбираем только нужное поле
                LinkEntities =
                    {
                       // Создаем связь с таблицей отношений
                       new LinkEntity(WorkGroup.EntityLogicalName,"fnt_territory_team", "teamid", "teamid", JoinOperator.Inner)
                       {
                           LinkCriteria = new FilterExpression
                           {
                               Conditions =
                               {
                                   // Правильное условие фильтрации по ID территории
                                   new ConditionExpression("fnt_territoryid", ConditionOperator.Equal, teamId)
                               }
                           }
                       }
                    }
            };

            // Выполняем запрос и получаем связанные территории
            var linkEntities = _service.RetrieveMultiple(query).Entities;

            List<Entity> territories = new List<Entity>();
            foreach (var linkEntity in linkEntities)
            {
                // Получаем идентификатор территории из связующей записи
                var territoryId = linkEntity.GetAttributeValue<EntityReference>("fnt_territoryid")?.Id;

                if (territoryId.HasValue)
                {
                    // Запрашиваем сущность territories для получения дополнительных данных о территории
                    var territory = _service.Retrieve("territory", territoryId.Value, new ColumnSet(true));
                    territories.Add(territory);
                }
            }

            return territories;
        }




        private List<Guid> GetTeamIdsByTerritory(Guid territoryId)
        {
            var teamIds = new List<Guid>();

            // Запрос для получения рабочих групп, связанных с определенной территорией через таблицу отношений
            QueryExpression teamQuery = new QueryExpression(WorkGroup.EntityLogicalName) // Сущность "Рабочая группа"
            {
                ColumnSet = new ColumnSet("teamid"), // Указываем, что хотим получить ID рабочих групп
                LinkEntities =
        {
            // Создаем связь с таблицей отношений
            new LinkEntity("fnt_territory_team", WorkGroup.EntityLogicalName, "teamid", "teamid", JoinOperator.Inner)
            {
                LinkCriteria = new FilterExpression
                {
                    Conditions =
                    {
                        // Правильное условие фильтрации по ID территории
                        new ConditionExpression("fnt_territoryid", ConditionOperator.Equal, territoryId)
                    }
                }
            }
        }
            };

            // Выполняем запрос к рабочим группам
            var teamEntities = _service.RetrieveMultiple(teamQuery).Entities;

            // Извлекаем ID рабочих групп из результатов запроса
            foreach (var entity in teamEntities)
            {
                // Добавляем ID рабочей группы в список
                teamIds.Add(entity.Id);
            }

            // Возвращаем список ID рабочих групп, связанных с указанной территорией
            return teamIds;
        }






        private List<Entity> GetProductBaskets(IOrganizationService service, Guid possibleDealId)
        {
            var query = new QueryExpression("productbasket")
            {
                ColumnSet = new ColumnSet("baseprice", "discount", "priceafterdiscount"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("possibledealid", ConditionOperator.Equal, possibleDealId)
                    }
                }
            };

            return service.RetrieveMultiple(query).Entities.ToList();
        }

        private void UpdatePossibleDeal(IOrganizationService service, Guid possibleDealId, decimal basePriceSum, decimal discountSum, decimal priceAfterDiscountSum)
        {
            var possibleDeal = new Entity("possibledeal", possibleDealId)
            {
                ["baseprice"] = new Money(basePriceSum),
                ["discount"] = new Money(discountSum),
                ["priceafterdiscount"] = new Money(priceAfterDiscountSum)
            };

            service.Update(possibleDeal);
        }
    }
}


