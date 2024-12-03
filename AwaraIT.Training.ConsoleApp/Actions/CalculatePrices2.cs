using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Web.Services.Description;

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
            try
            {
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

                // Получаем информацию о продукте
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

                QueryExpression quer1 = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                         {
                             new ConditionExpression(PriceListPositions.Metadata.PriceListPostionId, ConditionOperator.Equal, Guid.Parse("b66a97f3-83b1-ef11-b8e9-000d3a5c09a6")),

                         }
                    }
                };

                var priceListPositions = _service.RetrieveMultiple(quer1).Entities.FirstOrDefault().ToEntity<PriceListPositions>();
                var price = priceListPositions.Price.Value.ToString();
                var territoryRef = priceListPositions.TerritoryReference.Id.ToString();
                var priceListRef = priceListPositions.PriceListReference.Id.ToString();
                var formatPreparationRef = priceListPositions.FormatPreparationReference.Id.ToString();
                var formatConductionRef = priceListPositions.FormatConductionReference.Id.ToString();
                var subjectRef = priceListPositions.SubjectReference.Id.ToString();
                Console.WriteLine($"priceListRef {priceListRef}");
                Console.WriteLine($"price {price}");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine($"territoryRef {territoryRef}");
                Console.WriteLine($"territoryReference.Id.ToString()}} {territoryReference.Id.ToString()}");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine($"formatPreparationRef {formatPreparationRef}");
                Console.WriteLine($"formatPreparation.ToString() {formatPreparation.ToString()}");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine($"formatConductionRef {formatConductionRef}");
                Console.WriteLine($"formatConducting.ToString() {formatConducting.ToString()}");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine($"subjectRef {subjectRef}");
                Console.WriteLine($"subjectPreparation.ToString() {subjectPreparation.ToString()}");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");


                // Запрос к прайс листу для получения базовой цены
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

                var priceListPositionId = result.PriceListPostionId;
                var priceFinal = result.Price;
                var preceListReferen = result.PriceListReference;
                var territoryReferen = result.TerritoryReference;
                var formatPreprationRef = result.FormatPreparationReference;
                var formatConductRef = result.FormatConductionReference;
                var subjectRef1 = result.SubjectReference;
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine($"priceListPositionId {priceListPositionId}");
                Console.WriteLine($"priceFinal {priceFinal.Value.ToString()}");
                Console.WriteLine($"preceListReferen {preceListReferen.Id}");
                Console.WriteLine($"territoryReferen {territoryReferen.Id}");
                Console.WriteLine($"formatPreprationRef {formatPreprationRef.Id}");
                Console.WriteLine($"formatConductRef {formatConductRef.Id}");
                Console.WriteLine($"subjectRef1 {subjectRef1.Id}");




























                /*var req = new OrganizationRequest("fnt_CalculatePricingForPossibleDeal");
                req.Parameters.Add("PossibleDealId", possibleDealRef);
                req.Parameters.Add("ProductId", productRef);
                req.Parameters.Add("Discount", discount);
                var resp = _service.Execute(req);*/

                // Console.WriteLine($"Response: {resp.ToString()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }



        /*   public void Execute()
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
         }*/
    }
}

