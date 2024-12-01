using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс <c>Territory</c> представляет сущность территории в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class Territory : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Territory"/>.
        /// </summary>
        public Territory() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="Territory"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: имя территории.
            /// </summary>
            public const string TerritoryName = "fnt_territory_name";

            /// <summary>
            /// Логическое имя: идентификатора территории.
            /// </summary>
            public const string TerritoryID = "fnt_territoryid";
        }
        /// <summary>
        /// Свойство: идентификатор территории.
        /// </summary>
        public Guid TerritoryID
        {
            get { return GetAttributeValue<Guid>(Metadata.TerritoryID); }
            set { Attributes[Metadata.TerritoryID] = value; }
        }
        /// <summary>
        /// Логическое имя сущности территории.
        /// </summary>
        public const string EntityLogicalName = "fnt_territory";

        /// <summary>
        /// Свойство: имя территории.
        /// </summary>
        public string TerritoryName
        {
            get { return GetAttributeValue<string>(Metadata.TerritoryName); }
            set { Attributes[Metadata.TerritoryName] = value; }
        }
    }
}

