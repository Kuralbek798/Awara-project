using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities

{
    [EntityLogicalName(EntityLogicalName)]
    public class Teammembership : BaseEntity
    {

        public Teammembership() : base(EntityLogicalName) { }


        public const string EntityLogicalName = "teammembership";

        public static class Metadata
        {
            public const string TeamId = "teamid"; // Идентификатор команды
            public const string SystemUserId = "systemuserid"; // Идентификатор пользователя
        }

        // Свойства 
        public Guid TeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }

        public Guid SystemUserId
        {
            get { return GetAttributeValue<Guid>(Metadata.SystemUserId); }
            set { Attributes[Metadata.SystemUserId] = value; }
        }
    }
}
