using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Application.Core;
using System.Linq;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Класс <c>CalculatePrices</c> представляет действие в рабочем процессе CRM для расчета цен.
    /// </summary>
    public class CalculatePrices : BasicActivity
    {
        /// <summary>
        /// Входной параметр сделки.
        /// </summary>
        [Input("PossibleDealId")]
        [RequiredArgument]
        [ReferenceTarget(PosibleDeal.EntityLogicalName)]
        public InArgument<EntityReference> Deal { get; set; }

        /// <summary>
        /// Входной параметр продукта.
        /// </summary>
        [Input("ProductId")]
        [RequiredArgument]
        [ReferenceTarget(Product.EntityLogicalName)]
        public InArgument<EntityReference> ProductRef { get; set; }

        /// <summary>
        /// Входной параметр скидки.
        /// </summary>
        [Input("Discount")]
        public InArgument<Money> Discount { get; set; }

        /// <summary>
        /// Выходной параметр базовой цены.
        /// </summary>
        [Output("BasePrice")]
        public OutArgument<Money> BasePrice { get; set; }

        /// <summary>
        /// Выходной параметр цены со скидкой.
        /// </summary>
        [Output("DiscountedPrice")]
        public OutArgument<Money> DiscountedPrice { get; set; }

        private Logger _log;

        /// <summary>
        /// Метод, выполняющий логику действия.
        /// </summary>
        /// <param name="executionContext">Контекст выполнения действия.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            base.Execute(executionContext);
            // Получение входных параметров
            var dealEntityReference = Deal.Get(executionContext);
            var productReference = ProductRef.Get(executionContext);
            var discount = Discount.Get(executionContext);
            _log = new Logger(CrmService);
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Получаем информацию о возможной сделке
                var possibleDeal = service.Retrieve(dealEntityReference.LogicalName, dealEntityReference.Id, new ColumnSet(PosibleDeal.Metadata.TerritoryReference)).ToEntity<PosibleDeal>();
                var territoryReference = possibleDeal.TerritoryReference;

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

                var formatPreparation = productEntity.FormatPreparationReference;
                var formatConducting = productEntity.FormatConductionReference;
                var subjectPreparation = productEntity.SubjectPreparationReference;


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

                var result = service.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntity<PriceListPositions>();
                if (result == null)
                {
                    throw new InvalidPluginExecutionException("No data in price list.");
                }
                Money basePrice = result.Price;

                // Вычисление цены со скидкой
                Money discountedPrice = new Money(basePrice.Value - discount.Value);

                // Установка выходных параметров
                BasePrice.Set(executionContext, basePrice);
                DiscountedPrice.Set(executionContext, discountedPrice);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in customStep {nameof(CalculatePrices)} product info is null");
                throw new InvalidPluginExecutionException("Exception during calculation total price");
            }
        }
    }
}








