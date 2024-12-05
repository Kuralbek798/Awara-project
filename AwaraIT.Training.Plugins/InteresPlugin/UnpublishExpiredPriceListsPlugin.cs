using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Extensions;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Плагин для снятия с публикации прайс-листов, у которых истек срок действия.
    /// </summary>
    public class UnpublishExpiredPriceListsPlugin : PluginBase
    {
        private Logger _log;

        public UnpublishExpiredPriceListsPlugin() : base()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(PriceList.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(UnpublishExpiredPriceLists);

            Subscribe
                .ToMessage(CrmMessage.Update)
                .ForEntity(PriceList.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(UnpublishExpiredPriceLists);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который снимает с публикации прайс-листы с истекшим сроком действия.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void UnpublishExpiredPriceLists(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            try
            {
                // Запрос для получения всех прайс-листов с истекшим сроком действия и активным статусом
                var query = new QueryExpression(PriceList.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PriceList.Metadata.PriceListEndDate, PriceList.Metadata.StateCode),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            // Дата окончания меньше или равна текущей дате.
                            new ConditionExpression(PriceList.Metadata.PriceListEndDate, ConditionOperator.LessEqual, DateTime.UtcNow),
                            new ConditionExpression(PriceList.Metadata.StateCode, ConditionOperator.Equal, PriceList.StateCodeEnum.Active.ToIntValue())
                        }
                    }
                };

                var results = wrapper.Service.RetrieveMultiple(query);
                foreach (var entity in results.Entities)
                {
                    // Изменение статуса на "Неактивный"
                    entity[PriceList.Metadata.StateCode] = new OptionSetValue(PriceList.StateCodeEnum.Inactive.ToIntValue());
                    wrapper.Service.Update(entity);
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(UnpublishExpiredPriceLists)} of {nameof(UnpublishExpiredPriceListsPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(UnpublishExpiredPriceLists)} method of {nameof(UnpublishExpiredPriceListsPlugin)}.", ex);
            }
        }
    }
}





