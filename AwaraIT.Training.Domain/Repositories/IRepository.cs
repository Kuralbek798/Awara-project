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
    }
}
