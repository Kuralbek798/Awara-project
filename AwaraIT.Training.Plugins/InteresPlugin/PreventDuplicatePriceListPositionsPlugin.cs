using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Training.Application.Core;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Плагин для предотвращения создания дублирующихся позиций прайс-листа.
    /// </summary>
    public class PreventDuplicatePriceListPositionsPlugin : PluginBase
    {
        private Logger _log;

        public PreventDuplicatePriceListPositionsPlugin() : base()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(PriceListPositions.EntityLogicalName)
                .When(PluginStage.PreOperation)
                .Execute(PreventDuplicatePositions);

            Subscribe
               .ToMessage(CrmMessage.Update)
               .ForEntity(PriceListPositions.EntityLogicalName)
               .When(PluginStage.PreOperation)
               .Execute(PreventDuplicatePositions);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который предотвращает создание дублирующихся позиций прайс-листа.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void PreventDuplicatePositions(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            try
            {
                var entity = wrapper?.TargetEntity.ToEntity<PriceListPositions>();

                if (entity == null)
                {
                    _log.ERROR("Price list position is Null");
                    return;
                }

                var territory = entity.TerritoryReference?.Id;
                var preparationFormat = entity.FormatPreparationReference?.Id;
                var conductingFormat = entity.FormatConductionReference?.Id;
                var subject = entity.SubjectReference?.Id;

                var query = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PriceListPositions.Metadata.TerritoryReference,
                                              PriceListPositions.Metadata.FormatPreparationReference,
                                              PriceListPositions.Metadata.FormatConductionReference,
                                              PriceListPositions.Metadata.SubjectReference),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(PriceListPositions.Metadata.TerritoryReference, ConditionOperator.Equal, territory),
                            new ConditionExpression(PriceListPositions.Metadata.FormatPreparationReference, ConditionOperator.Equal, preparationFormat),
                            new ConditionExpression(PriceListPositions.Metadata.FormatConductionReference, ConditionOperator.Equal, conductingFormat),
                            new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal, subject)
                        }
                    }
                };

                var results = wrapper.Service.RetrieveMultiple(query);
                if (results.Entities.Count > 0)
                {
                    _log.ERROR("A price list position with the same combination already exists.");
                    throw new InvalidPluginExecutionException("Прайс-лист с подобными данными уже присутствует в таблице, измените входные параметры вашего прайс-листа");
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(PreventDuplicatePositions)} of {nameof(PreventDuplicatePriceListPositionsPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(PreventDuplicatePositions)} method of {nameof(PreventDuplicatePriceListPositionsPlugin)}.", ex);
            }
        }
    }
}

