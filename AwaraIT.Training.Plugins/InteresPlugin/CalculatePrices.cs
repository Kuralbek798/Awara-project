using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using System.Linq;
using AwaraIT.Training.Application.Core;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PriceList;
using AwaraIT.Training.Domain.Extensions;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Представляет настраиваемую активность рабочего процесса для расчета цен в CRM.
    /// </summary>
    public class CalculatePrices : NativeActivity
    {
        /// <summary>
        /// Получает или задает идентификатор возможной сделки.
        /// </summary>
        [RequiredArgument]
        [Input("PossibleDealId")]
        [ReferenceTarget(PossibleDeal.EntityLogicalName)]
        public InArgument<EntityReference> PossibleDealId { get; set; }

        /// <summary>
        /// Получает или задает идентификатор продукта.
        /// </summary>
        [RequiredArgument]
        [Input("ProductId")]
        [ReferenceTarget(Product.EntityLogicalName)]
        public InArgument<EntityReference> ProductId { get; set; }

        /// <summary>
        /// Получает или задает скидку.
        /// </summary>
        [Input("Discount")]
        public InArgument<Money> Discount { get; set; }

        /// <summary>
        /// Получает или задает базовую цену.
        /// </summary>
        [Output("BasePrice")]
        public OutArgument<Money> BasePrice { get; set; }

        /// <summary>
        /// Получает или задает цену со скидкой.
        /// </summary>
        [Output("DiscountedPrice")]
        public OutArgument<Money> DiscountedPrice { get; set; }

        private Logger _log;

        /// <summary>
        /// Выполняет настраиваемую активность рабочего процесса для расчета цен.
        /// </summary>
        /// <param name="context">Контекст выполнения.</param>
        protected override void Execute(NativeActivityContext context)
        {
            var workflowContext = context.GetExtension<IWorkflowContext>();
            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            var tracingService = context.GetExtension<ITracingService>();

            _log = new Logger(service);
            _log.INFO("CalculatePrices started");
            tracingService.Trace("CalculatePrices started");

            // Получение входных параметров
            var dealEntityReference = PossibleDealId.Get(context);
            var productReference = ProductId.Get(context);
            var discount = Discount.Get(context);

            try
            {
                _log.INFO($"CalculatePrices dealEntityReference received: {dealEntityReference?.Id}");
                _log.INFO($"CalculatePrices productReference received: {productReference?.Id}");
                _log.INFO($"CalculatePrices discount received: {discount?.Value}");

                if (dealEntityReference == null || productReference == null || discount == null)
                {
                    _log.ERROR("Один или несколько входных параметров равны null");
                    tracingService.Trace("Один или несколько входных параметров равны null");
                    throw new InvalidPluginExecutionException("Один или несколько входных параметров равны null");
                }

                // Запрос информации о сделке
                var possibleDeal = service.Retrieve(dealEntityReference.LogicalName, dealEntityReference.Id,
                                                    new ColumnSet(PossibleDeal.Metadata.TerritoryReference)).ToEntity<PossibleDeal>();
                var territoryReference = possibleDeal.TerritoryReference;
                _log.INFO($"Territory Reference received: {territoryReference?.Id}");

                if (territoryReference == null || territoryReference.Id == Guid.Empty)
                {
                    _log.ERROR($"Ошибка в customStep {nameof(CalculatePrices)}: территория равна null");
                    tracingService.Trace($"Ошибка в customStep {nameof(CalculatePrices)}: территория равна null");
                    throw new InvalidPluginExecutionException("Не удалось получить территорию из возможной сделки");
                }

                // Запрос информации о продукте
                var productEntity = service.Retrieve(productReference.LogicalName, productReference.Id,
                                                     new ColumnSet(Product.Metadata.FormatPreparationReference,
                                                                   Product.Metadata.FormatConductionReference,
                                                                   Product.Metadata.SubjectPreparationReference)).ToEntity<Product>();
                if (productEntity == null)
                {
                    _log.ERROR($"Ошибка в customStep {nameof(CalculatePrices)}: информация о продукте равна null");
                    tracingService.Trace($"Ошибка в customStep {nameof(CalculatePrices)}: информация о продукте равна null");
                    throw new InvalidPluginExecutionException("Информация о продукте равна null");
                }

                var formatPreparation = productEntity.FormatPreparationReference?.Id;
                var formatConducting = productEntity.FormatConductionReference?.Id;
                var subjectPreparation = productEntity.SubjectPreparationReference?.Id;

                _log.INFO($"Format Preparation: {formatPreparation}");
                _log.INFO($"Format Conducting: {formatConducting}");
                _log.INFO($"Subject Preparation: {subjectPreparation}");



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
                            new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal, subjectPreparation),
                            new ConditionExpression(PriceList.Metadata.StateCode, ConditionOperator.Equal, StateCodeEnum.Active.ToIntValue() )
                        }
                    }
                };





                // Запрос позиции прайс-листа
                //QueryExpression query = new QueryExpression(PriceListPositions.EntityLogicalName)
                //{
                //    ColumnSet = new ColumnSet(PriceListPositions.Metadata.Price),
                //    Criteria = new FilterExpression
                //    {
                //        Conditions =
                //             {
                //                 new ConditionExpression(PriceListPositions.Metadata.TerritoryReference, ConditionOperator.Equal, territoryReference.Id),
                //                 new ConditionExpression(PriceListPositions.Metadata.FormatPreparationReference, ConditionOperator.Equal, formatPreparation),
                //                 new ConditionExpression(PriceListPositions.Metadata.FormatConductionReference, ConditionOperator.Equal, formatConducting),
                //                 new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal, subjectPreparation)
                //             }
                //    }
                //};

                var result = service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<PriceListPositions>();
                if (result == null)
                {
                    _log.ERROR($"Ошибка в customStep {nameof(CalculatePrices)}: нет данных в прайс-листе.");
                    tracingService.Trace("Нет данных в прайс-листе.");
                    throw new InvalidPluginExecutionException("Нет данных в прайс-листе.");
                }
                Money basePrice = result.Price;
                _log.INFO($"Base price received {basePrice.Value}");

                // Расчет скидки
                Money discountedPrice = new Money(basePrice.Value - discount.Value);
                _log.INFO($"Discounted price received {discountedPrice.Value}");

                // Установка выходных параметров
                BasePrice.Set(context, basePrice);
                DiscountedPrice.Set(context, discountedPrice);
                _log.INFO($"BasePrice {BasePrice.ToString()}, DiscountedPrice {DiscountedPrice.ToString()}");

            }
            catch (Exception ex)
            {
                _log.ERROR(ex, $"Ошибка в customStep {nameof(CalculatePrices)}");
                throw new InvalidPluginExecutionException("Исключение при расчете общей цены: " + ex.Message, ex);
            }
        }
    }
}




