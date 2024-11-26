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

namespace AwaraIT.Training.Plugins.InteresPlugin
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
                .Execute(Execute);
        }

        public void Execute(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);

            Interest preImage = wrapper.Context?.PreEntityImages["image"].ToEntity<Interest>();
            Interest postImage = wrapper.Context?.PostEntityImages["image"].ToEntity<Interest>();



            // Получение PreImage и PostImage


            if (preImage == null && postImage == null)
            {
                _log.ERROR("PreImage and PostImage objects are Null");
                return;
            }

            var interest = postImage ?? preImage;

            if (interest == null)
            {
                _log.ERROR("Interest object is Null");
                return;
            }

            try
            {
                if (interest.LogicalName == Interest.EntityLogicalName)
                {
                    if (interest.StatusEnum == InterestStepStatus.Agreement)
                    {
                        var contactReference = interest.ContactReference;
                        var territoryReference = interest.TerritoryReference;

                        if (contactReference != null)
                        {
                            _log.INFO($"Contact Reference ID: {contactReference.Id}");
                        }
                        else
                        {
                            _log.WARNING("Contact Reference is Null.");
                        }

                        if (territoryReference != null)
                        {
                            _log.INFO($"Territory Reference ID: {territoryReference.Id}");
                        }
                        else
                        {
                            _log.WARNING("Territory Reference is Null.");
                        }

                        // Создание новой записи в сущности "Возможная сделка"
                        var possibleDealEntity = new Entity(PosibleDeal.EntityLogicalName)
                        {
                            [PosibleDeal.Metadata.ContactReference] = contactReference,
                            [PosibleDeal.Metadata.Status] = new OptionSetValue(PosibleDealStatusEnums.Open.ToIntValue()),
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
                else
                {
                    _log.ERROR("Interest object has incorrect logical name.");
                }
            }
            catch (Exception ex)
            {
                _log.ERROR(ex, "Error in CreatePossibleDealOnInterestApprovalPlugin");
                throw new Exception($"Error occurred in {nameof(CreatePossibleDealOnInterestApprovalPlugin)}: {ex.Message}");
            }
        }
    }
}
