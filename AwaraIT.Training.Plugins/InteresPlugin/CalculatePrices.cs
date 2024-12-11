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
using AwaraIT.Kuralbek.Plugins.Helpers;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Представляет настраиваемую активность рабочего процесса для расчета цен в CRM.
    /// </summary>

    public class CalculatePrices : NativeActivity //Не знаю почему но в CodeActivity не работает Logger поэтому использую NativeActivity
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


            _log = new Logger(service);
            _log.INFO("CalculatePrices started");


            // Получение входных параметров
            var possibleDealReference = PossibleDealId.Get(context);
            var productReference = ProductId.Get(context);
            var discount = Discount.Get(context);

            try
            {

                _log.INFO($"CalculatePrices possibleDealReference received: {possibleDealReference?.Id}");
                _log.INFO($"CalculatePrices productReference received: {productReference?.Id}");
                _log.INFO($"CalculatePrices discount received: {discount?.Value}");

                if (possibleDealReference == null || productReference == null || discount == null)
                {
                    _log.ERROR("One or some incoming parameters are null");

                    throw new InvalidPluginExecutionException("One or some incoming parameters are null");
                }
                var columnSet = PluginHelper.CreateColumnSet(PossibleDeal.Metadata.TerritoryReference);
                var territoryReference = PluginHelper.ValidateEntityReference(GetPossibleDeal(service, possibleDealReference, columnSet).ToEntity<PossibleDeal>().TerritoryReference);
                _log.INFO($"Territory Reference received: {territoryReference?.Id}");

                // Запрос информации о сделке


                // Запрос информации о продукте
                var productEntity = service.Retrieve(productReference.LogicalName, productReference.Id,
                                                     new ColumnSet(Product.Metadata.FormatPreparationReference,
                                                                   Product.Metadata.FormatConductionReference,
                                                                   Product.Metadata.SubjectPreparationReference)).ToEntity<Product>();
                if (productEntity == null)
                {
                    _log.ERROR($"Error in customStep {nameof(CalculatePrices)}: product information is null");
                    throw new InvalidPluginExecutionException("product information is null");
                }

                var formatPreparation = productEntity.FormatPreparationReference?.Id;
                var formatConducting = productEntity.FormatConductionReference?.Id;
                var subjectPreparation = productEntity.SubjectPreparationReference?.Id;

                _log.INFO($"Format Preparation: {formatPreparation}");
                _log.INFO($"Format Conducting: {formatConducting}");
                _log.INFO($"Subject Preparation: {subjectPreparation}");

                // Запрос к прайс-листу
                QueryExpression query = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PriceListPositions.Metadata.Price),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                          new ConditionExpression(PriceListPositions.Metadata.TerritoryReference, ConditionOperator.Equal, territoryReference.Id),
                          new ConditionExpression(PriceListPositions.Metadata.FormatPreparationReference, ConditionOperator.Equal, formatPreparation),
                          new ConditionExpression(PriceListPositions.Metadata.FormatConductionReference, ConditionOperator.Equal, formatConducting),
                          new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal, subjectPreparation)
                        },
                    },
                    LinkEntities =
                    {
                       new LinkEntity(PriceListPositions.EntityLogicalName, PriceList.EntityLogicalName, PriceListPositions.Metadata.PriceListReference, PriceList.Metadata.PriceListId, JoinOperator.Inner)
                       {
                         LinkCriteria = new FilterExpression
                         {
                            FilterOperator = LogicalOperator.And,
                           Conditions =
                           {
                             new ConditionExpression(PriceList.Metadata.StateCode, ConditionOperator.Equal, StateCodeEnum.Active.ToIntValue()),
                             new ConditionExpression(PriceList.Metadata.PriceListEndDate, ConditionOperator.GreaterEqual, DateTime.UtcNow)
                           }
                         }
                       }
                    }
                };

                var result = service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<PriceListPositions>();

                if (result == null)
                {
                    _log.ERROR($"Error in customStep {nameof(CalculatePrices)}: no data in price-list");
                    throw new InvalidPluginExecutionException("no data in price-list");
                }
                Money basePrice = result.Price;
                _log.INFO($"Base price received {basePrice.Value}");

                // Расчет скидки.
                Money discountedPrice = new Money(basePrice.Value - discount.Value);
                _log.INFO($"Discounted price received {discountedPrice.Value}");

                // Установка выходных параметров
                BasePrice.Set(context, basePrice);
                DiscountedPrice.Set(context, discountedPrice);
                _log.INFO($"BasePrice {BasePrice.Get(context)?.Value}, DiscountedPrice {DiscountedPrice.Get(context)?.Value}");

            }
            catch (Exception ex)
            {
                _log.ERROR(ex, $"Error in customStep {nameof(CalculatePrices)}");
                throw new InvalidPluginExecutionException("There is an exception on calculating total price: " + ex.Message, ex);
            }
        }

        private Entity GetPossibleDeal(IOrganizationService service, EntityReference entityReference, ColumnSet columnSet)
        {
            try
            {
                entityReference = PluginHelper.ValidateEntityReference(entityReference);

                var possibleDeal = service.Retrieve(entityReference.LogicalName, entityReference.Id, columnSet);
                return possibleDeal;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}






