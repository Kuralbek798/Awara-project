using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{

    [EntityLogicalName(EntityLogicalName)]
    public class Patern : BaseEntity
    {
        public Patern() : base(EntityLogicalName) { }
        public static class Metadata
        {
            public const string Str = "";
        }
        public const string EntityLogicalName = "";

        public Guid Str
        {
            get { return GetAttributeValue<Guid>(Metadata.Str); }
            set { Attributes[Metadata.Str] = value; }
        }
    }
}
