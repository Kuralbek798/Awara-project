using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{


    [EntityLogicalName(EntityLogicalName)]
    public class TerritoryTeamNN : BaseEntity
    {

        public TerritoryTeamNN() : base(EntityLogicalName) { }

        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: ProductCartId - записи 
            /// в связующей таблице между территорией и командой
            /// </summary>
            public const string TerritoryTeamId = "fnt_territory_teamid";
            /// <summary>
            /// Логическое имя: ProductCartId - Идентификатор территории
            /// </summary>
            public const string TerritoryId = "fnt_territoryid";

            /// <summary>
            /// Логическое имя: ProductCartId -  Идентификатор команды
            /// </summary>
            public const string TeamId = "teamid";
        }
        /// <summary>
        /// таблица связи между территорией и команой.
        /// </summary>
        public const string EntityLogicalName = "fnt_territory_team";

        /// <summary>
        /// Свойство: связующей таблице между территорией и командой
        /// </summary>
        public Guid TerritoryTeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TerritoryTeamId); }
            set { Attributes[Metadata.TerritoryTeamId] = value; }
        }
        /// <summary>
        /// Свойство:  Идентификатор территории
        /// </summary>
        public Guid TerritoryId
        {
            get { return GetAttributeValue<Guid>(Metadata.TerritoryId); }
            set { Attributes[Metadata.TerritoryId] = value; }
        }
        /// <summary>
        /// Свойство:   Идентификатор команды.
        /// </summary>
        public Guid TeamId
        {
            get { return GetAttributeValue<Guid>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }
    }
}
