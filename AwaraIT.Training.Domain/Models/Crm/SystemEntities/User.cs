using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    [EntityLogicalName(EntityLogicalName)]
    public class User : BaseEntity
    {
        public User() : base(EntityLogicalName) { }

        public static class Metadata
        {

            public const string SystemUserId = "systemuserid";

        }

        public const string EntityLogicalName = "systemuser";


        public Guid SystemUserId
        {
            get { return GetAttributeValue<Guid>(Metadata.SystemUserId); }
            set { Attributes[Metadata.SystemUserId] = value; }
        }

    }
}
