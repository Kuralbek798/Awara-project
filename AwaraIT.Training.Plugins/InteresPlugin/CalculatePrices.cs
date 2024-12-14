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
using AwaraIT.Training.Domain.Repositories;
using AwaraIT.Kuralbek.Plugins.Hellpers;

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
        private EntityReference territoryReference;

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
            IRepository repository = new Repository(service);
            _log.INFO("CalculatePrices started");

            try
            {
                // Get input parameters
                var possibleDealReference = PossibleDealId.Get(context);
                var productReference = ProductId.Get(context);
                var discount = Discount.Get(context);

                // Validate input parameters
                PluginHelper.ValidateEntityReferencesWithTuples(_log,
                    (possibleDealReference, nameof(CalculatePrices), nameof(possibleDealReference)),
                    (productReference, nameof(CalculatePrices), nameof(productReference))
                );
                DataForLogs.SaveInputParametersLogs(_log, possibleDealReference.Id.ToString(), productReference.Id.ToString(), discount.Value.ToString());

                // Get territory reference
                territoryReference = GetTerritoryReference(possibleDealReference, repository);
                _log.INFO($"Territory Reference received: {territoryReference?.Id}");

                // Get product details
                var productInfo = GetProductDetailsGuids(productReference, repository);
                DataForLogs.SaveProductDeatailsLogs(_log, productInfo.formatPreparationId.ToString(), productInfo.formatConductingId.ToString(), productInfo.subjectPreparationId.ToString());

                // Get Base Price
                Money basePrice = GetBasePrice(territoryReference.Id, productInfo.formatPreparationId, productInfo.formatConductingId, productInfo.subjectPreparationId, repository);

                // Calculate Discounted Price
                Money discountedPrice = CalculateDiscountedPrice(basePrice, discount);

                // Set prices to output parameters
                SetPricesToOutputParameters(basePrice, discountedPrice, context);
            }
            catch (Exception ex)
            {
                _log.ERROR(ex, $"Error in customStep {nameof(CalculatePrices)}");
                throw new InvalidPluginExecutionException("There is an exception on calculating total price: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Получает ссылку на территорию из возможной сделки.
        /// </summary>
        /// <param name="possibleDealReference">Ссылка на возможную сделку.</param>
        /// <param name="repository">Репозиторий для доступа к данным.</param>
        /// <returns>Ссылка на территорию.</returns>
        private EntityReference GetTerritoryReference(EntityReference possibleDealReference, IRepository repository)
        {
            try
            {
                // Create column set for possible deal
                var columnSetForPossibleDeal = PluginHelper.CreateColumnSet(false, PossibleDeal.Metadata.TerritoryReference);
                // Get territory reference
                territoryReference = repository.GetEntityDataByReference(possibleDealReference, columnSetForPossibleDeal).ToEntity<PossibleDeal>().TerritoryReference;
                // Validate territory reference
                territoryReference = PluginHelper.ValidateEntityReference(territoryReference, nameof(CalculatePrices), nameof(territoryReference), _log);

                return territoryReference;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in GetTerritoryReference: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// Получает детали продукта.
        /// </summary>
        /// <param name="productReference">Ссылка на продукт.</param>
        /// <param name="repository">Репозиторий для доступа к данным.</param>
        /// <returns>Кортеж с идентификаторами формата подготовки, формата проведения и предмета подготовки.</returns>
        private (Guid formatPreparationId, Guid formatConductingId, Guid subjectPreparationId) GetProductDetailsGuids(EntityReference productReference, IRepository repository)
        {
            // Create column set for product
            var columnSetForProduct = PluginHelper.CreateColumnSet(
                false,
                Product.Metadata.FormatPreparationReference,
                Product.Metadata.FormatConductionReference,
                Product.Metadata.SubjectPreparationReference
            );
            // Get product details
            var productEntity = repository.GetEntityDataByReference(productReference, columnSetForProduct).ToEntity<Product>();

            var formatPreparation = productEntity.FormatPreparationReference;
            var formatConducting = productEntity.FormatConductionReference;
            var subjectPreparation = productEntity.SubjectPreparationReference;
            // Validate product details
            PluginHelper.ValidateEntityReferencesWithTuples(_log,
                (formatPreparation, nameof(CalculatePrices), nameof(formatPreparation)),
                (formatConducting, nameof(CalculatePrices), nameof(formatConducting)),
                (subjectPreparation, nameof(CalculatePrices), nameof(subjectPreparation))
            );

            return (formatPreparation.Id, formatConducting.Id, subjectPreparation.Id);
        }

        /// <summary>
        /// Получает базовую цену из прайс-листа.
        /// </summary>
        /// <param name="territoryId">Идентификатор территории.</param>
        /// <param name="formatPreparationId">Идентификатор формата подготовки.</param>
        /// <param name="formatConductingId">Идентификатор формата проведения.</param>
        /// <param name="subjectPreparationId">Идентификатор предмета подготовки.</param>
        /// <param name="repository">Репозиторий для доступа к данным.</param>
        /// <returns>Базовая цена.</returns>
        private Money GetBasePrice(Guid territoryId, Guid formatPreparationId, Guid formatConductingId, Guid subjectPreparationId, IRepository repository)
        {
            var result = repository.GetPrice(territoryId, formatPreparationId, formatConductingId, subjectPreparationId).Entities.FirstOrDefault().ToEntity<PriceListPositions>();
            if (result == null)
            {
                _log.ERROR($"Error in customStep {nameof(CalculatePrices)}: no data in price-list");
                throw new InvalidPluginExecutionException("no data in price-list");
            }
            _log.INFO($"Base price received {result.Price.Value}");
            return result.Price;
        }

        /// <summary>
        /// Рассчитывает цену со скидкой.
        /// </summary>
        /// <param name="basePrice">Базовая цена.</param>
        /// <param name="discount">Скидка.</param>
        /// <returns>Цена со скидкой.</returns>
        private Money CalculateDiscountedPrice(Money basePrice, Money discount)
        {
            var discountedPrice = new Money(basePrice.Value - discount.Value);
            _log.INFO($"Discounted price received {discountedPrice.Value}");
            return discountedPrice;
        }

        /// <summary>
        /// Устанавливает базовую цену и цену со скидкой в выходные параметры.
        /// </summary>
        /// <param name="basePrice">Базовая цена.</param>
        /// <param name="discountedPrice">Цена со скидкой.</param>
        /// <param name="context">Контекст выполнения.</param>
        private void SetPricesToOutputParameters(Money basePrice, Money discountedPrice, NativeActivityContext context)
        {
            BasePrice.Set(context, basePrice);
            DiscountedPrice.Set(context, discountedPrice);
            _log.INFO($"BasePrice {BasePrice.Get(context)?.Value}, DiscountedPrice {DiscountedPrice.Get(context)?.Value}");
        }
    }
}
