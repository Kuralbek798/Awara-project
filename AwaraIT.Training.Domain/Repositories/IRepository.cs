using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Repositories
{
    public interface IRepository
    {
        Entity GetEntityDataByReference(EntityReference entityReference, ColumnSet columnSet);
        EntityCollection GetPrice(Guid territoryId, Guid formatPreparationId, Guid formatConductingId, Guid subjectPreparationId);
        Guid Create(Entity entity);
        DataCollection<Entity> GetInfoOnMultipleRetrive(string entityLogicalName, ColumnSet columnSet, List<ConditionExpression> conditionExpressions = null, LinkEntity linkEntity = null);
    }
}
