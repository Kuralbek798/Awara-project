using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    /// <summary>
    /// Класс <c>TeammembershipNN</c> представляет связь N:N между командой и пользователем в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class TeammembershipNN : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TeammembershipNN"/>.
        /// </summary>
        public TeammembershipNN() : base(EntityLogicalName) { }

        /// <summary>
        /// Логическое имя: таблицы связи между командой и пользователем.
        /// </summary>
        public const string EntityLogicalName = "teammembership";

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="TeammembershipNN"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: идентификатор записи в связующей таблице между командой и пользователем.
            /// </summary>
            public const string TeamMembershipId = "teammembershipid";
            /// <summary>
            /// Логическое имя: ссылка на команду.
            /// </summary>
            public const string TeamId = "teamid";
            /// <summary>
            /// Логическое имя: ссылка на пользователя.
            /// </summary>
            public const string SystemUserId = "systemuserid";
        }

        /// <summary>
        /// Свойство: идентификатор записи в связующей таблице между командой и пользователем.
        /// </summary>
        public Guid TeamMembershipId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamMembershipId); }
            set { Attributes[Metadata.TeamMembershipId] = value; }
        }
        /// <summary>
        /// Свойство: ссылка на команду.
        /// </summary>
        public Guid TeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }
        /// <summary>
        /// Свойство: ссылка на пользователя.
        /// </summary>
        public Guid SystemUserId
        {
            get { return GetAttributeValue<Guid>(Metadata.SystemUserId); }
            set { Attributes[Metadata.SystemUserId] = value; }
        }
    }
}

