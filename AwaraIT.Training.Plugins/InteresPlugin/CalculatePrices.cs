using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Application.Core;

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
        public InArgument<decimal> Discount { get; set; }

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
        Logger _log;
        /// <summary>
        /// Метод, выполняющий логику действия.
        /// </summary>
        /// <param name="executionContext">Контекст выполнения действия.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            base.Execute(executionContext);
            //Получение входных параметров
            var dealEntityReference = Deal.Get(executionContext);
            var productReference = ProductRef.Get(executionContext);
            var discount = Discount.Get(executionContext);
            _log = new Logger(CrmService);
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Получаем территорию из сделки
                var territoryReference = service.Retrieve(dealEntityReference.LogicalName, dealEntityReference.Id, new ColumnSet(PosibleDeal.Metadata.TerritoryReference)).ToEntity<PosibleDeal>().TerritoryReference;
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

                Guid formatPreparation = productEntity.EducationTypeReference;
                Guid formatConducting = productEntity.EventTypeReference;
                Guid subjectPreparation = productEntity.SubjectReference;

                // Запрос к прайс листу для получения базовой цены
                QueryExpression query = new QueryExpression("pricelevel")
                {
                    ColumnSet = new ColumnSet("baseprice"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("territory", ConditionOperator.Equal, territoryReference.Id),
                            new ConditionExpression("formatpreparation", ConditionOperator.Equal, formatPreparation),
                            new ConditionExpression("formatconducting", ConditionOperator.Equal, formatConducting),
                            new ConditionExpression("subjectpreparation", ConditionOperator.Equal, subjectPreparation)
                        }
                    }
                };

                var results = service.RetrieveMultiple(query);
                if (results.Entities.Count == 0)
                {
                    throw new InvalidPluginExecutionException("Не удалось найти запись в прайс-листе для указанных параметров.");
                }
                decimal basePrice = results.Entities[0].GetAttributeValue<Money>("baseprice")?.Value ?? 0;

                // Вычисление цены со скидкой
                decimal discountedPrice = basePrice - discount;

                // Установка выходных параметров
                BasePrice.Set(executionContext, new Money(basePrice));
                DiscountedPrice.Set(executionContext, new Money(discountedPrice));
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Ошибка при расчете цен: " + ex.Message);
            }
        }
    }
}

