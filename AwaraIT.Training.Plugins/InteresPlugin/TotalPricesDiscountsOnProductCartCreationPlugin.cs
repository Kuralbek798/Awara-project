using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Training.Application.Core;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Плагин для расчета полей цен в сделке при создании продуктовой корзины.
    /// </summary>
    public class TotalPricesDiscountsOnProductCartCreationPlugin : PluginBase
    {
        private Logger _log;

        public TotalPricesDiscountsOnProductCartCreationPlugin() : base()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(ProductCart.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(Calculate);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который рассчитывает поля цен в сделке.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void Calculate(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            try
            {
                var productCart = wrapper?.TargetEntity.ToEntity<ProductCart>();

                if (productCart == null)
                {
                    _log.ERROR("Product cart is Null");
                    return;
                }


                var possibleDealId = GetPossibleDealId(wrapper.Service, productCart.Id);

                if (possibleDealId == null)
                {
                    _log.ERROR("Possible deal ID is Null");
                    return;
                }

                // Запрос для получения всех продуктовых корзин, связанных с возможной сделкой
                QueryExpression query = new QueryExpression(ProductCart.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(ProductCart.Metadata.Price, ProductCart.Metadata.Discount, ProductCart.Metadata.PriceAfterDiscount),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                       {
                           new ConditionExpression(ProductCart.Metadata.PossibleDealReference, ConditionOperator.Equal, possibleDealId)
                       }
                    }
                };

                var productCarts = wrapper.Service.RetrieveMultiple(query).Entities
                                                  .Select(e => e.ToEntity<ProductCart>())
                                                  .ToList();

                // Суммирование значений полей цен в сделке
                decimal totalBasePrice = productCarts.Sum(pc => pc.Price?.Value ?? 0);
                decimal totalDiscount = productCarts.Sum(pc => pc.Discount?.Value ?? 0);
                decimal totalPriceAfterDiscount = productCarts.Sum(pc => pc.PriceAfterDiscount?.Value ?? 0);

                // Обновление сущности PossibleDeal
                Entity possibleDeal = new Entity(PossibleDeal.EntityLogicalName, possibleDealId.Value)
                {
                    [PossibleDeal.Metadata.Price] = new Money(totalBasePrice),
                    [PossibleDeal.Metadata.Discount] = new Money(totalDiscount),
                    [PossibleDeal.Metadata.PriceAfterDiscount] = new Money(totalPriceAfterDiscount)
                };

                wrapper.Service.Update(possibleDeal);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(Calculate)} of {nameof(TotalPricesDiscountsOnProductCartCreationPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(Calculate)} method of {nameof(TotalPricesDiscountsOnProductCartCreationPlugin)}.", ex);
            }
        }

        /// <summary>
        /// Получает идентификатор возможной сделки из связи N:N.
        /// </summary>
        /// <param name="service">Сервис организации.</param>
        /// <param name="productCartId">Идентификатор продуктовой корзины.</param>
        /// <returns>Идентификатор возможной сделки.</returns>
        private Guid? GetPossibleDealId(IOrganizationService service, Guid productCartId)
        {

            var productCart = service.Retrieve(ProductCart.EntityLogicalName, productCartId,
                                               new ColumnSet(ProductCart.Metadata.PossibleDealReference)).ToEntity<ProductCart>();

            if (productCart != null && productCart.Contains(ProductCart.Metadata.PossibleDealReference))
            {
                return productCart.PossibleDealReference.Id;
            }

            return null;
        }


    }
}





