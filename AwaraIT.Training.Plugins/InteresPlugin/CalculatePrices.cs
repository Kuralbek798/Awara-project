using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Training.Domain.Models.Crm.Entities;

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
        [ReferenceTarget("opportunity")]
        public InArgument<EntityReference> Deal { get; set; }

        /// <summary>
        /// Входной параметр продукта.
        /// </summary>
        [Input("ProductId")]
        [ReferenceTarget("product")]
        public InArgument<EntityReference> Product { get; set; }

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

        /// <summary>
        /// Метод, выполняющий логику действия.
        /// </summary>
        /// <param name="executionContext">Контекст выполнения действия.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            base.Execute(executionContext);
            //Получение входных параметров
            var deal = Deal.Get(executionContext);
            var product = Product.Get(executionContext);
            var discount = Discount.Get(executionContext);

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Получаем территорию из сделки
                var dealEntity = service.Retrieve(deal.LogicalName, deal.Id, new ColumnSet("territory")).ToEntity<PosibleDeal>();
                if (dealEntity == null || !dealEntity.Contains("territory"))
                {
                    throw new InvalidPluginExecutionException("Не удалось получить территорию из возможной сделки.");
                }
                var territoryRef = dealEntity.TerritoryReference; //.GetAttributeValue<EntityReference>("territory");

                // Получаем информацию о продукте
                var productEntity = service.Retrieve(product.LogicalName, product.Id, new ColumnSet("formatpreparation", "formatconducting", "subjectpreparation"));
                if (productEntity == null)
                {
                    throw new InvalidPluginExecutionException("Не удалось получить информацию о продукте.");
                }
                string formatPreparation = productEntity.GetAttributeValue<string>("formatpreparation");
                string formatConducting = productEntity.GetAttributeValue<string>("formatconducting");
                string subjectPreparation = productEntity.GetAttributeValue<string>("subjectpreparation");

                // Запрос к прайс листу для получения базовой цены
                QueryExpression query = new QueryExpression("pricelevel");
                query.ColumnSet = new ColumnSet("baseprice");
                query.Criteria.AddCondition("territory", ConditionOperator.Equal, territoryRef.Id);
                query.Criteria.AddCondition("formatpreparation", ConditionOperator.Equal, formatPreparation);
                query.Criteria.AddCondition("formatconducting", ConditionOperator.Equal, formatConducting);
                query.Criteria.AddCondition("subjectpreparation", ConditionOperator.Equal, subjectPreparation);

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
