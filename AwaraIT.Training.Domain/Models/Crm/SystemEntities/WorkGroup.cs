using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    [EntityLogicalName(EntityLogicalName)]
    public class WorkGroup : BaseEntity
    {
        public WorkGroup() : base(EntityLogicalName) { }

        public static class Metadata
        {

            public const string TeamId = "teamid";
            public const string Name = "name";

        }

        public const string EntityLogicalName = "team";

        public Guid TeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }
        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }

    }
}
