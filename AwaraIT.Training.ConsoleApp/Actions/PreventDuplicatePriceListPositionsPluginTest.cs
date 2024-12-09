using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    public class PreventDuplicatePriceListPositionsPluginTest
    {
        public static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {
                    var clientD365 = (IOrganizationService)client;
                    var test = new Test33(clientD365);
                    test.Execute();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }
    }

    internal class Test33
    {
        private Logger _log;
        private readonly IOrganizationService _service;
        public Test33(IOrganizationService service)
        {
            _service = service;
            Execute();
        }

        public void Execute()
        {
            _log = new Logger(_service);

            try
            {


                var territory = Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6");
                var preparationFormat = Guid.Parse("3ad3a8b7-8aa9-ef11-b8e9-000d3a5c09a6");
                var conductingFormat = Guid.Parse("42c60b9d-91a9-ef11-b8e9-000d3a5c09a6");
                var subject = Guid.Parse("37c7a063-7da9-ef11-b8e9-000d3a5c09a6");
                var priceListReference = Guid.Parse("43c8cb1b-81b1-ef11-b8e9-000d3a5c09a6");


                var query = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PriceListPositions.Metadata.TerritoryReference,
                                              PriceListPositions.Metadata.FormatPreparationReference,
                                              PriceListPositions.Metadata.FormatConductionReference,
                                              PriceListPositions.Metadata.SubjectReference),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(PriceListPositions.Metadata.TerritoryReference, ConditionOperator.Equal, territory),
                            new ConditionExpression(PriceListPositions.Metadata.FormatPreparationReference, ConditionOperator.Equal, preparationFormat),
                            new ConditionExpression(PriceListPositions.Metadata.FormatConductionReference, ConditionOperator.Equal, conductingFormat),
                            new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal, subject),
                            new ConditionExpression(PriceListPositions.Metadata.PriceListReference,ConditionOperator.Equal, priceListReference)
                        }
                    }
                };

                var results = _service.RetrieveMultiple(query);
                if (results.Entities.Count > 0)
                {
                    _log.ERROR("A price list position with the same combination already exists.");
                    // Вызов Action для передачи параметров ошибки
                    var request = new OrganizationRequest("new_priceListPositionsDuplicateInfo")
                    {
                        ["ErrorMessage"] = "Error Duplicate!"
                    };
                    var sdf = _service.Execute(request);
                    Console.WriteLine(sdf);

                    //throw new Exception("Error Duplicate!") { HResult };
                }
            }
            catch (Exception ex)
            {
                // _log.ERROR($"Error in method {nameof(PreventDuplicatePositions)} of {nameof(PreventDuplicatePriceListPositionsPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(Execute)} method of {nameof(Execute)}.", ex);
            }
        }




    }
}



