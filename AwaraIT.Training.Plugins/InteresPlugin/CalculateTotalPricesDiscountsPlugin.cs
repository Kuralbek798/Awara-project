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
    public class CalculateTotalPricesDiscountsPlugin : PluginBase
    {
        private Logger _log;

        public CalculateTotalPricesDiscountsPlugin() : base()
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

                //запрос для получения идентификатора возможной сделки
                var possibleDealId = GetPossibleDealId(wrapper.Service, productCart.Id);

                if (possibleDealId == null)
                {
                    _log.ERROR("Possible deal ID is Null");
                    return;
                }

                // Запрос для получения продуктовых корзин, связанных с возможной сделкой
                QueryExpression query = new QueryExpression(ProductCart.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(ProductCart.Metadata.Price, ProductCart.Metadata.Discount, ProductCart.Metadata.PriceAfterDiscount),
                    LinkEntities =
                    {
                        new LinkEntity(ProductCart.EntityLogicalName, PossibleDealProductCartNN.EntityLogicalName, ProductCart.Metadata.ProductCartId, PossibleDealProductCartNN.Metadata.ProductCartId, JoinOperator.Inner)
                        {
                            LinkCriteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression(PossibleDealProductCartNN.Metadata.PossibleDealId, ConditionOperator.Equal, possibleDealId)
                                }
                            }
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

                //Добавление сумм в сущность PossibleDeal
                totalBasePrice += productCart.Price?.Value ?? 0;
                totalDiscount += productCart.Discount?.Value ?? 0;
                totalPriceAfterDiscount += productCart.PriceAfterDiscount?.Value ?? 0;

                // Обновление сущности PossibleDeal
                Entity possibleDeal = new Entity(PosibleDeal.EntityLogicalName, possibleDealId.Value)
                {
                    [PosibleDeal.Metadata.Price] = new Money(totalBasePrice),
                    [PosibleDeal.Metadata.Discount] = new Money(totalDiscount),
                    [PosibleDeal.Metadata.PriceAfterDiscount] = new Money(totalPriceAfterDiscount)
                };

                wrapper.Service.Update(possibleDeal);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(Calculate)} of {nameof(CalculateTotalPricesDiscountsPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(Calculate)} method of {nameof(CalculateTotalPricesDiscountsPlugin)}.", ex);
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
            QueryExpression query = new QueryExpression(PossibleDealProductCartNN.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(PossibleDealProductCartNN.Metadata.PossibleDealId),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(PossibleDealProductCartNN.Metadata.ProductCartId, ConditionOperator.Equal, productCartId)
                    }
                }
            };

            EntityCollection results = service.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
            {
                return results.Entities.First().GetAttributeValue<Guid>(PossibleDealProductCartNN.Metadata.PossibleDealId);
            }

            return null;
        }
    }
}

