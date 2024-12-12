using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Extensions;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PriceList;

namespace AwaraIT.Training.Domain.Repositories
{
    public class Repository : IRepository
    {
        private readonly IOrganizationService _service;
        private Logger _log;

        public Repository(IOrganizationService service)
        {
            _service = service;
            _log = new Logger(service);
        }
        public Guid Create(Entity entity)
        {
            try
            {
                return _service.Create(entity);
            }
            catch (Exception ex)
            {

                _log.ERROR($"Error ocured in {nameof(Create)}: {ex.Message}");
                throw ex;
            }
        }
        public Entity GetEntityDataByReference(EntityReference entityReference, ColumnSet columnSet)
        {
            try
            {
                return _service.Retrieve(entityReference.LogicalName, entityReference.Id, columnSet);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error ocured in {nameof(GetEntityDataByReference)}: {ex.Message}");
                throw ex;
            }
        }
        #region CalculatePrices
        public EntityCollection GetPrice(Guid territoryId, Guid formatPreparationId, Guid formatConductingId, Guid subjectPreparationId)
        {
            try
            {
                QueryExpression query = new QueryExpression(PriceListPositions.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(PriceListPositions.Metadata.Price),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                          new ConditionExpression(PriceListPositions.Metadata.TerritoryReference, ConditionOperator.Equal, territoryId),
                          new ConditionExpression(PriceListPositions.Metadata.FormatPreparationReference, ConditionOperator.Equal, formatPreparationId),
                          new ConditionExpression(PriceListPositions.Metadata.FormatConductionReference, ConditionOperator.Equal, formatConductingId),
                          new ConditionExpression(PriceListPositions.Metadata.SubjectReference, ConditionOperator.Equal,subjectPreparationId)
                        },
                    },
                    LinkEntities =
                        {
                           new LinkEntity(PriceListPositions.EntityLogicalName, PriceList.EntityLogicalName, PriceListPositions.Metadata.PriceListReference, PriceList.Metadata.PriceListId, JoinOperator.Inner)
                           {
                             LinkCriteria = new FilterExpression
                             {
                                FilterOperator = LogicalOperator.And,
                               Conditions =
                               {
                                 new ConditionExpression(PriceList.Metadata.StateCode, ConditionOperator.Equal, StateCodeEnum.Active.ToIntValue()),
                                 new ConditionExpression(PriceList.Metadata.PriceListEndDate, ConditionOperator.GreaterEqual, DateTime.UtcNow)
                               }
                             }
                           }
                        }
                };
                return _service.RetrieveMultiple(query);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in {nameof(GetPrice)}: {ex.Message}");
                throw ex;
            }
        }
        #endregion
    }
}
