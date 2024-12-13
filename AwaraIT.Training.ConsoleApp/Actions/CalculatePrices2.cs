using AwaraIT.Kuralbek.Plugins.Hellpers;
using AwaraIT.Kuralbek.Plugins.Helpers;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Training.Domain.Repositories;
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
                    test.StartMethod();
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
        private CodeActivityContext _context;

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
            _context = context;
            StartMethod();
        }
        private EntityReference territoryReference;


        public void StartMethod()
        {
            var possibleDealRef = new EntityReference("fnt_posible_deal", Guid.Parse("90E5FE56-79B1-EF11-B8E9-000D3A5C09A6"));
            var productRef = new EntityReference("fnt_education_product", Guid.Parse("CB88AE4D-5EAA-EF11-B8E8-00224808BD77"));
            //var productRef = productCart.ProductReference;


            IRepository repository = new Repository(_service);


            try
            {
                // Get input parameters
                var possibleDealReference = possibleDealRef;
                var productReference = productRef;
                var discount = new Money(33);

                // Validate input parametera
                ConsolePluginHelper.ValidateEntityReferencesWithTuples(
                    (possibleDealReference, nameof(CalculatePrices2), nameof(possibleDealReference)),
                    (productReference, nameof(CalculatePrices2), nameof(productReference))
                );


                // Get territory reference
                territoryReference = GetTerritoryReference(possibleDealReference, repository);


                // Get product details
                var productInfo = GetProductDetailsGuids(productReference, repository);

                // Get Base Price
                Money basePrice = GetBasePrice(territoryReference.Id, productInfo.formatPreparationId, productInfo.formatConductingId, productInfo.subjectPreparationId, repository);

                // Calculate Discounted Price
                Money discountedPrice = CalculateDiscountedPrice(basePrice, discount);

                // Set prices to output parameters
                SetPricesToOutputParameters(basePrice, discountedPrice, _context);
            }
            catch (Exception ex)
            {

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
                var columnSetForPossibleDeal = ConsolePluginHelper.CreateColumnSet(PossibleDeal.Metadata.TerritoryReference);
                // Get territory reference
                territoryReference = repository.GetEntityDataByReference(possibleDealReference, columnSetForPossibleDeal).ToEntity<PossibleDeal>().TerritoryReference;
                // Validate territory reference
                territoryReference = ConsolePluginHelper.ValidateEntityReference(territoryReference, nameof(CalculatePrices2), nameof(territoryReference));

                return territoryReference;
            }
            catch (Exception ex)
            {

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
            var columnSetForProduct = ConsolePluginHelper.CreateColumnSet(
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
            ConsolePluginHelper.ValidateEntityReferencesWithTuples(
                (formatPreparation, nameof(CalculatePrices2), nameof(formatPreparation)),
                (formatConducting, nameof(CalculatePrices2), nameof(formatConducting)),
                (subjectPreparation, nameof(CalculatePrices2), nameof(subjectPreparation))
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

                throw new InvalidPluginExecutionException("no data in price-list");
            }

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

            return discountedPrice;
        }

        /// <summary>
        /// Устанавливает базовую цену и цену со скидкой в выходные параметры.
        /// </summary>
        /// <param name="basePrice">Базовая цена.</param>
        /// <param name="discountedPrice">Цена со скидкой.</param>
        /// <param name="context">Контекст выполнения.</param>
        private void SetPricesToOutputParameters(Money basePrice, Money discountedPrice, CodeActivityContext context)
        {
            BasePrice.Set(context, basePrice);
            DiscountedPrice.Set(context, discountedPrice);

        }

    }
}
