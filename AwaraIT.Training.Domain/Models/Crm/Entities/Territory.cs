using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Territory : BaseEntity
    {
        public Territory() : base(EntityLogicalName) { }

        public static class Metadata
        {

            public const string TerritoryName = "fnt_territory_name";
            public const string TerritoryID = "fnt_territoryid";

        }

        public const string EntityLogicalName = "fnt_territory";


        public string TerritoryName
        {
            get { return GetAttributeValue<string>(Metadata.TerritoryName); }
            set { Attributes[Metadata.TerritoryName] = value; }
        }
         public string TerritoryID
        {
            get { return GetAttributeValue<string>(Metadata.TerritoryID); }
            set { Attributes[Metadata.TerritoryID] = value; }
        }

    }
}
