using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    [EntityLogicalName(EntityLogicalName)]
    public class User : BaseEntity
    {
        public User() : base(EntityLogicalName) { }

        public static class Metadata
        {
            /// <summary>
            /// Логическое имя:  идентификатор пользователя.
            /// </summary>
            public const string SystemUserId = "systemuserid";

        }
        /// <summary>
        /// Логическое имя: таблицы пользователя.
        /// </summary>
        public const string EntityLogicalName = "systemuser";

        /// <summary>
        /// Свойство:  идентификатор пользователя.
        /// </summary>
        public Guid SystemUserId
        {
            get { return GetAttributeValue<Guid>(Metadata.SystemUserId); }
            set { Attributes[Metadata.SystemUserId] = value; }
        }

    }
}
