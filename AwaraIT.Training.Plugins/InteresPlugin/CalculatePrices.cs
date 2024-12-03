using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Training.Domain.Models.Crm.Entities;

using System.Linq;
using AwaraIT.Training.Application.Core;





namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Класс <c>CalculatePrices</c> представляет действие в рабочем процессе CRM для расчета цен.
    /// </summary>
    public class CalculatePrices : CodeActivity
    {
        [RequiredArgument]
        [Input("PossibleDealId")]
        [ReferenceTarget(PossibleDeal.EntityLogicalName)]
        public InArgument<EntityReference> PossibleDealId { get; set; }
        // public InArgument<string> PossibleDealId { get; set; }

        [RequiredArgument]
        [Input("ProductId")]
        [ReferenceTarget(Product.EntityLogicalName)]
        public InArgument<EntityReference> ProductId { get; set; }
        //public InArgument<string> ProductId { get; set; }

        [Input("Discount")]
        public InArgument<Money> Discount { get; set; }


        [Output("BasePrice")]
        public OutArgument<Money> BasePrice { get; set; }


        [Output("DiscountedPrice")]
        public OutArgument<Money> DiscountedPrice { get; set; }

        private Logger _log;
        protected override void Execute(CodeActivityContext context)
        {
            var workflowContext = context.GetExtension<IWorkflowContext>();
            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            _log = new Logger(service);
            _log.INFO("CalculatePrices started");

            // Получение входных параметров
            var dealEntityReference = PossibleDealId.Get(context);
            var productReference = ProductId.Get(context);
            var discount = Discount.Get(context);

            try
            {
                _log.INFO($"CalculatePrices dealEntityReference received: {dealEntityReference.Id.ToString()}");
                _log.INFO($"CalculatePrices productReference received: {productReference.Id.ToString()}");
                _log.INFO($"CalculatePrices discount received: {discount?.Value.ToString()}");


                if (dealEntityReference == null || productReference == null || discount == null)
                {
                    _log.ERROR("One or more input parameters are null");
                    throw new InvalidPluginExecutionException("One or more input parameters are null");
                }

                // Получаем информацию о возможной сделке
                var possibleDeal = service.Retrieve(dealEntityReference.LogicalName, dealEntityReference.Id,
                                                    new ColumnSet(PossibleDeal.Metadata.TerritoryReference)).ToEntity<PossibleDeal>();
                var territoryReference = possibleDeal.TerritoryReference;
                _log.INFO($"Territory Reference received: {territoryReference?.Id}");

                if (territoryReference == null || territoryReference.Id == Guid.Empty)
                {
                    _log.ERROR($"Error in customStep {nameof(CalculatePrices)} territory is null");
                    throw new InvalidPluginExecutionException("Failed to retrieve territory from possible deal");
                }

                // Получаем информацию о продукте
                var productEntity = service.Retrieve(productReference.LogicalName, productReference.Id,
                                                     new ColumnSet(Product.Metadata.FormatPreparationReference,
                                                                   Product.Metadata.FormatConductionReference,
                                                                   Product.Metadata.SubjectPreparationReference)).ToEntity<Product>();
                if (productEntity == null)
                {
                    _log.ERROR($"Error in customStep {nameof(CalculatePrices)} product info is null");
                    throw new InvalidPluginExecutionException("Product info is null");
                }

                var formatPreparation = productEntity.FormatPreparationReference.Id;
                var formatConducting = productEntity.FormatConductionReference.Id;
                var subjectPreparation = productEntity.SubjectPreparationReference.Id;

                // Запрос к прайс листу для получения базовой цены
                QueryExpression query = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PriceListPositions.Metadata.Price),
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

                var result = service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<PriceListPositions>();
                if (result == null)
                {
                    _log.ERROR($"Error in customStep {nameof(CalculatePrices)} No data in price list.");
                    throw new InvalidPluginExecutionException("No data in price list.");
                }
                Money basePrice = result.Price;
                _log.INFO($"Base price received {basePrice.Value}");

                // Вычисление цены со скидкой
                Money discountedPrice = new Money(basePrice.Value - discount.Value);
                _log.INFO($"Discounted price received {discountedPrice.Value}");

                // Установка выходных параметров
                BasePrice.Set(context, basePrice);
                DiscountedPrice.Set(context, discountedPrice);
            }
            catch (Exception ex)
            {
                _log.ERROR(ex, $"Error in customStep {nameof(CalculatePrices)}");
                throw new InvalidPluginExecutionException("Exception during calculation total price" + ex.Message, ex);
            }
        }
    }
}
