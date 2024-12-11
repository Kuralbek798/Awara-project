using AwaraIT.Training.Application.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Repositories
{
    public class Repository : IRepository
    {
        private readonly IOrganizationService _service;
        private Logger _log;

        public Repository(IOrganizationService service, Logger logger)
        {
            _service = service;
            _log = logger;
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
    }
}
