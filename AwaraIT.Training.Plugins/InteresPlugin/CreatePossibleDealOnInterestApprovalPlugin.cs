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
using static AwaraIT.Training.Domain.Models.Crm.Entities.PosibleDeal;
using AwaraIT.Training.Domain.Extensions;
using System.IdentityModel.Protocols.WSTrust;

namespace AwaraIT.Kuralbek.Plugins.InteresPlugin
{
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
            _log = new Logger(wrapper.Service);

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

                    if (contactReference != null)
                    {
                        _log.INFO($"Contact Reference ID: {contactReference.Id}");
                    }
                    else
                    {
                        _log.ERROR("Contact Reference is Null.");
                        throw new Exception("contactReference is null");
                    }

                    if (territoryReference != null)
                    {
                        _log.INFO($"Territory Reference ID: {territoryReference.Id}");
                    }
                    else
                    {
                        _log.ERROR("Territory Reference is Null.");
                        throw new Exception("territoryReference is null");
                    }

                    // Создание новой записи в сущности "Возможная сделка"
                    var possibleDealEntity = new Entity(PosibleDeal.EntityLogicalName)
                    {
                        [PosibleDeal.Metadata.ContactReference] = contactReference,
                        [PosibleDeal.Metadata.Status] = new OptionSetValue(PosibleDealStepStatus.Open.ToIntValue()),
                        [PosibleDeal.Metadata.TerritoryReference] = territoryReference
                    };

                    var dealId = wrapper.Service.Create(possibleDealEntity);
                    _log.INFO($"Created Possible Deal with ID: {dealId}");
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
    }
}

