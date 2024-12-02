/*using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    public class Test1232
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

    public class Test
    {
        IOrganizationService _service;

        public Test(IOrganizationService service)
        {
            _service = service;
            Execute();
        }

        public void Execute()
        {
            var productReferenceId = Guid.Parse("ba505fa4-a7ac-ef11-b8e9-000d3a5c09a6");

            QueryExpression query = new QueryExpression(PosibleDeal.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(PosibleDeal.Metadata.TerritoryReference),
                LinkEntities =
                {
                    new LinkEntity(PosibleDeal.EntityLogicalName, PossibleDealProductCartNN.EntityLogicalName, PosibleDeal.Metadata.PosibleDealId, PossibleDealProductCartNN.Metadata.PossibleDealId, JoinOperator.Inner)
                    {
                        LinkEntities =
                        {
                            new LinkEntity(PossibleDealProductCartNN.EntityLogicalName, ProductCart.EntityLogicalName, PossibleDealProductCartNN.Metadata.ProductCartId, ProductCart.Metadata.ProductCartId, JoinOperator.Inner)
                            {
                                Columns = new ColumnSet(ProductCart.Metadata.ProductReference, ProductCart.Metadata.Price, ProductCart.Metadata.Discount),
                                EntityAlias = "productCart",
                                LinkCriteria = new FilterExpression
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression(ProductCart.Metadata.ProductCartId, ConditionOperator.Equal, productReferenceId)
                                    }
                                }
                            }
                        }
                    }
                },
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(PosibleDeal.Metadata.PosibleDealId, ConditionOperator.Equal, productReferenceId)
                    }
                }
            };

            var result = _service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (result != null)
            {
                var dealId = result.GetAttributeValue<Guid>(PosibleDeal.Metadata.PosibleDealId);
                var territory = result.GetAttributeValue<EntityReference>(PosibleDeal.Metadata.TerritoryReference);
                var productCartId = result.GetAttributeValue<AliasedValue>("productCart." + ProductCart.Metadata.ProductCartId)?.Value as Guid?;
                var productFormatPreparation = result.GetAttributeValue<AliasedValue>("productCart." + ProductCart.Metadata.ProductReference)?.Value as string;
                var productFormatConduction = result.GetAttributeValue<AliasedValue>("productCart." + ProductCart.Metadata.Price)?.Value as Money;
                var productSubjectPreparation = result.GetAttributeValue<AliasedValue>("productCart." + ProductCart.Metadata.Discount)?.Value as Money;

                Console.WriteLine($"DealId: {dealId}");
                Console.WriteLine($"Territory: {territory?.Id}");
                Console.WriteLine($"ProductCartId: {productCartId}");
                Console.WriteLine($"ProductFormatPreparation: {productFormatPreparation}");
                Console.WriteLine($"ProductFormatConduction: {productFormatConduction}");
                Console.WriteLine($"ProductSubjectPreparation: {productSubjectPreparation}");
            }
            else
            {
                Console.WriteLine("No matching records found.");
            }
        }
    }
}

*/