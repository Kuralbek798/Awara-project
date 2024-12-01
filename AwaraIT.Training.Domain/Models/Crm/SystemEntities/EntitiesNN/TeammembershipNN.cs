using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{


    [EntityLogicalName(EntityLogicalName)]
    public class TeammembershipNN : BaseEntity
    {
        public TeammembershipNN() : base(EntityLogicalName) { }

        /// <summary>
        /// Логическое имя: таблицы связи между командой и пользователем.
        /// </summary>
        public const string EntityLogicalName = "teammembership";

        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: ProductCartId - Идентификатор записи 
            /// в связующей таблице между командой и пользователем
            /// </summary>
            public const string TeamMembershipId = "teammembershipid";
            /// <summary>
            /// Логическое имя: ProductCartId - ссылка на команду
            /// </summary>
            public const string TeamId = "teamid";
            /// <summary>
            /// Логическое имя: ProductCartId -  ссылка на пользователя
            /// </summary>
            public const string SystemUserId = "systemuserid";
        }

        /// <summary>
        /// Свойство: ProductCartId - связь между командой и пользователем
        /// </summary>
        public Guid TeamMembershipId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamMembershipId); }
            set { Attributes[Metadata.TeamMembershipId] = value; }
        }
        /// <summary>
        /// Свойство: ProductCartId - ссылка на команду
        /// </summary>
        public Guid TeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }
        /// <summary>
        /// Свойство: ProductCartId - ссылка на пользователя
        /// </summary>
        public Guid SystemUserId
        {
            get { return GetAttributeValue<Guid>(Metadata.SystemUserId); }
            set { Attributes[Metadata.SystemUserId] = value; }
        }
    }
}
