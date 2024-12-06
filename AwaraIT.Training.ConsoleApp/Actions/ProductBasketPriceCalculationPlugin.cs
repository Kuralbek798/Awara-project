
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.Collections.Generic;
using AwaraIT.Kuralbek.Plugins.Actions;

using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
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





            GetUsersByTerritoryId(Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));




            //GetTeamsByTerritory(Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));
            // GetTestUsersWithTerritory(_service);

        }


        private List<Guid> GetUsersByTerritoryId(Guid territoryId)
        {


            User user = new User();
            user.SystemUserId = Guid.Empty;

            try
            {

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
                                        Conditions =
                                        {
                                            new ConditionExpression(TerritoryTeamNN.Metadata.TerritoryId, ConditionOperator.Equal, territoryId)
                                        }
                                    }
                                }
                            }
                        }
                    }
                };


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






















    }
}


