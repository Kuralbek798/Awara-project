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
using AwaraIT.Kuralbek.Plugins.InteresPlugin;
using AwaraIT.Training.Domain.Models.Crm.DTO;

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

                // Получаем все продуктовые корзины, связанные с делкой
                QueryExpression query = new QueryExpression(ProductCart.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(ProductCart.Metadata.Price, ProductCart.Metadata.Discount, ProductCart.Metadata.PriceAfterDiscount),
                    LinkEntities =
                    {
                        new LinkEntity(ProductCart.EntityLogicalName, PossibleDealProductCartNN.EntityLogicalName, ProductCart.Metadata.ProductCartId, PossibleDealProductCartNN.Metadata.ProductCartId, JoinOperator.Inner)
                        {
                            LinkEntities =
                            {
                                new LinkEntity(PossibleDealProductCartNN.EntityLogicalName, PosibleDeal.EntityLogicalName, PossibleDealProductCartNN.Metadata.PossibleDealId, PosibleDeal.Metadata.PosibleDealId, JoinOperator.Inner)
                                {
                                    Columns = new ColumnSet(PosibleDeal.Metadata.PosibleDealId),
                                    EntityAlias = PosibleDeal.EntityAlias
                                }
                            }
                        }
                    }
                };

                var productCartsDTO = wrapper.Service.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<ProductCartDTO>()).ToList();

                if (productCartsDTO.Count == 0)
                {
                    _log.WARNING("No product carts found for the possible deal.");
                    return;
                }

                // Подсчет сумм баззовой цены, скидки и цены после скидки
                decimal totalBasePrice = productCartsDTO.Sum(pc => pc.Price?.Value ?? 0);
                decimal totalDiscount = productCartsDTO.Sum(pc => pc.Discount?.Value ?? 0);
                decimal totalPriceAfterDiscount = productCartsDTO.Sum(pc => pc.PriceAfterDiscount?.Value ?? 0);

                // Получаем идентификатор возможной сделки
                Guid possibleDealId = productCartsDTO.FirstOrDefault().PossibleDealId ?? Guid.Empty;

                if (possibleDealId == Guid.Empty)
                {
                    return;
                }

                // обновляем поля в возможной сделке
                Entity possibleDeal = new Entity(PosibleDeal.EntityLogicalName, possibleDealId)
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
    }
}
