using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    public class CalculatePrices2
    {
        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {
                    var clientD365 = (IOrganizationService)client;
                    var test = new Test(clientD365);
                    test.Execute();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }
    }

    public class Test : CodeActivity
    {
        private CodeActivityContext context;

        [Output("BasePrice")]
        public OutArgument<Money> BasePrice { get; set; }

        [Output("DiscountedPrice")]
        public OutArgument<Money> DiscountedPrice { get; set; }

        IOrganizationService _service;

        public Test(IOrganizationService service)
        {
            _service = service;
        }

        protected override void Execute(CodeActivityContext context)
        {
            this.context = context;
            Execute();
        }

        public void Execute()
        {
            /*   var workflowContext = context.GetExtension<IWorkflowContext>();
               var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
               var service = serviceFactory.CreateOrganizationService(workflowContext.UserId);*/


            try
            {
                Console.WriteLine("Starting Execute method...");

                var productCart = _service.Retrieve(ProductCart.EntityLogicalName, Guid.Parse("9bbbdbcb-84b1-ef11-b8e9-000d3a5c09a6"), new ColumnSet(true)).ToEntity<ProductCart>();
                var possibleDealRef = productCart.PossibleDealReference;
                var productRef = productCart.ProductReference;
                var discount = productCart.Discount;

                if (possibleDealRef == null || productRef == null || discount == null)
                {
                    Console.WriteLine("One or more input parameters are null");
                    return;
                }

                var possibleDeal = _service.Retrieve(possibleDealRef.LogicalName, possibleDealRef.Id,
                                            new ColumnSet(PossibleDeal.Metadata.TerritoryReference)).ToEntity<PossibleDeal>();

                var territoryReference = possibleDeal.TerritoryReference;

                if (territoryReference == null || territoryReference.Id == Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("Failed to retrieve territory from possible deal");
                }

                var productEntity = _service.Retrieve(productRef.LogicalName, productRef.Id,
                                                     new ColumnSet(Product.Metadata.FormatPreparationReference,
                                                                   Product.Metadata.FormatConductionReference,
                                                                   Product.Metadata.SubjectPreparationReference)).ToEntity<Product>();
                if (productEntity == null)
                {
                    throw new InvalidPluginExecutionException("Product info is null");
                }

                var formatPreparation = productEntity.FormatPreparationReference.Id;
                var formatConducting = productEntity.FormatConductionReference.Id;
                var subjectPreparation = productEntity.SubjectPreparationReference.Id;

                QueryExpression query = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                             {
                                 new ConditionExpression(PriceListPositions.Metadata.TerritoryReference, ConditionOperator.Equal, territoryReference.Id),
                                 new ConditionExpression(PriceListPositions.Metadata.FormatPreparationReference, ConditionOperator.Equal, formatPreparation),
                                 new ConditionExpression(PriceListPositions.Metadata.FormatConductionReference, ConditionOperator.Equal, formatConducting),
                                 new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal, subjectPreparation)
                             }
                    }
                };

                var result = _service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<PriceListPositions>();
                if (result == null)
                {
                    throw new InvalidPluginExecutionException("No data in price list.");
                }

                Money basePrice = result.Price;
                Money discountedPrice = new Money(basePrice.Value - discount.Value);

                Console.WriteLine($"BasePrice: {basePrice.Value}");
                Console.WriteLine($"DiscountedPrice: {discountedPrice.Value}");

                if (context != null)
                {
                    BasePrice.Set(context, basePrice);
                    DiscountedPrice.Set(context, discountedPrice);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
