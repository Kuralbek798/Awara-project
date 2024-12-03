/*using AwaraIT.Training.Domain.Models.Crm.DTO;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwaraIT.Training.Application.Core;

namespace AwaraIT.Kuralbek.Plugins.Actions
{

    public static class CalculateTotalPricesDiscountsPluginTest
    {

        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {

                    var clietntD365 = (IOrganizationService)client;

                    var tsPl = new Test3(clietntD365);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");

            }
        }
    }


    public class Test3
    {
        private Logger _log;

        public Test3(IOrganizationService wrapper) : base()
        {
            Calculate(wrapper);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который рассчитывает поля цен в сделке.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void Calculate(IOrganizationService wrapper)
        {
            _log = new Logger(wrapper);

            try
            {
                *//*var productCart = wrapper?.ToEntity<ProductCart>();*//*

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
                                new LinkEntity(PossibleDealProductCartNN.EntityLogicalName, PossibleDeal.EntityLogicalName, PossibleDealProductCartNN.Metadata.PossibleDealId, PossibleDeal.Metadata.PosibleDealId, JoinOperator.Inner)
                                {
                                    Columns = new ColumnSet(PossibleDeal.Metadata.PosibleDealId),
                                    EntityAlias = PossibleDeal.EntityAlias
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
                Entity possibleDeal = new Entity(PossibleDeal.EntityLogicalName, possibleDealId)
                {
                    [PossibleDeal.Metadata.Price] = new Money(totalBasePrice),
                    [PossibleDeal.Metadata.Discount] = new Money(totalDiscount),
                    [PossibleDeal.Metadata.PriceAfterDiscount] = new Money(totalPriceAfterDiscount)
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

}
*/