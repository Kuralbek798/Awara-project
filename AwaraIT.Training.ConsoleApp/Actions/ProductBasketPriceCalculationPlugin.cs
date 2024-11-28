/*
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.Collections.Generic;

namespace AwaraIT.Kuralbek.Plugins

{
    public class ProductBasketPriceCalculationPlugin 
    {
        private readonly Logger _log;

        public ProductBasketPriceCalculationPlugin() : base()
        {
            Subscribe
                .ToMessage("Create")
                .ToEntity("productbasket")
                .ToStage(PluginStage.PostOperation)
                .WithHandler(Execute);
        }

        private void Execute(IContextWrapper context)
        {
            _log = new Logger(context.Service);
            var productBasket = context?.TargetEntity;

            if (productBasket == null || !productBasket.Contains("possibledealid"))
                return;

            var possibleDealId = ((EntityReference)productBasket["possibledealid"]).Id;

            try
            {
                // Retrieve all product baskets related to the possible deal
                var productBaskets = GetProductBaskets(context.Service, possibleDealId);

                // Calculate the sums
                decimal basePriceSum = productBaskets.Sum(pb => pb.GetAttributeValue<Money>("baseprice")?.Value ?? 0);
                decimal discountSum = productBaskets.Sum(pb => pb.GetAttributeValue<Money>("discount")?.Value ?? 0);
                decimal priceAfterDiscountSum = productBaskets.Sum(pb => pb.GetAttributeValue<Money>("priceafterdiscount")?.Value ?? 0);

                // Update the possible deal
                UpdatePossibleDeal(context.Service, possibleDealId, basePriceSum, discountSum, priceAfterDiscountSum);
            }
            catch (Exception ex)
            {
                _log.ERROR(ex, "Error in ProductBasketPriceCalculationPlugin");
                throw;
            }
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
*/