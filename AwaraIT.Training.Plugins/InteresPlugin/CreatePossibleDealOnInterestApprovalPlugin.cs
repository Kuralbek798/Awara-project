using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.IdentityModel.Protocols.WSTrust;
using AwaraIT.Kuralbek.Plugins.Helpers;
using AwaraIT.Training.Domain.Repositories;

namespace AwaraIT.Kuralbek.Plugins.InteresPlugin
{
    /// <summary>
    /// Плагин для создания записи "Возможная сделка" при изменении статуса интереса на "Согласование".
    /// </summary>
    public class CreatePossibleDealOnInterestApprovalPlugin : PluginBase
    {
        private Logger _log;

        public CreatePossibleDealOnInterestApprovalPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Update)
                .ForEntity(Interest.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(CreatePossibleDeal);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который создает запись "Возможная сделка" при изменении статуса интереса на "Согласование".
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        public void CreatePossibleDeal(IContextWrapper wrapper)
        {
            var service = wrapper.Service;
            _log = new Logger(service);
            IRepository repository = new Repository(service);
            var interest = wrapper.PreImage.ToEntity<Interest>();
            var target = wrapper.TargetEntity.ToEntity<Interest>();

            if (interest == null)
            {
                _log.ERROR("Interest object is Null");
                return;
            }

            try
            {
                if (target.StatusToEnum == InterestStepStatus.Agreement)
                {
                    var contactReference = interest.ContactReference;
                    var territoryReference = interest.TerritoryReference;
                    PluginHelper.ValidateEntityReferencesWithTuples(_log, (contactReference, nameof(CreatePossibleDealOnInterestApprovalPlugin), nameof(contactReference)),
                                                                         (territoryReference, nameof(CreatePossibleDealOnInterestApprovalPlugin), nameof(territoryReference)));

                    CreateNewPossibleDeal(contactReference, territoryReference, repository);
                }
                else
                {
                    _log.INFO("Interest status is not Agreement.");
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(CreatePossibleDeal)} of {nameof(CreatePossibleDealOnInterestApprovalPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(CreatePossibleDeal)} method of {nameof(CreatePossibleDealOnInterestApprovalPlugin)}.", ex);
            }
        }

        /// <summary>
        /// Создает новую запись "Возможная сделка".
        /// </summary>
        /// <param name="contactReference">Ссылка на контакт.</param>
        /// <param name="territoryReference">Ссылка на территорию.</param>
        /// <param name="repository">Репозиторий для создания записи.</param>
        /// <exception cref="InvalidPluginExecutionException">Выбрасывается при возникновении ошибки во время создания записи.</exception>
        private void CreateNewPossibleDeal(EntityReference contactReference, EntityReference territoryReference, IRepository repository)
        {
            try
            {
                // Create new possible deal
                var possibleDealEntity = new Entity(PossibleDeal.EntityLogicalName)
                {
                    [PossibleDeal.Metadata.ContactReference] = contactReference,
                    [PossibleDeal.Metadata.Status] = new OptionSetValue(PossibleDealStepStatus.Open.ToIntValue()),
                    [PossibleDeal.Metadata.TerritoryReference] = territoryReference
                };
                var dealId = repository.Create(possibleDealEntity);
                _log.INFO($"Created Possible Deal with ID: {dealId}");
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(CreateNewPossibleDeal)} of {nameof(CreatePossibleDealOnInterestApprovalPlugin)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(CreateNewPossibleDeal)} method of {nameof(CreatePossibleDealOnInterestApprovalPlugin)}.", ex);
            }
        }
    }
}
