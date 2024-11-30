using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Team : BaseEntity
    {
        public Team() : base(EntityLogicalName) { }

        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Id - идентификатор команды.
            /// </summary>
            public const string TeamId = "teamid";
            /// <summary>
            /// Логическое имя: название рабочей группы.
            /// </summary>
            public const string Name = "name";

        }
        /// <summary>
        /// Логическое имя: таблицы пользователя.
        /// </summary>
        public const string EntityLogicalName = "team";

        public Guid TeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }

        /// <summary>
        /// Свойство: название рабочей группы.
        /// </summary>
        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }

    }
}
