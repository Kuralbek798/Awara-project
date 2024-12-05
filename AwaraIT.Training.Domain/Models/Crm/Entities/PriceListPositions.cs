using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Представляет сущность позиции прайс-листа в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class PriceListPositions : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PriceListPositions"/>.
        /// </summary>
        public PriceListPositions() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="PriceListPositions"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Идентификатор позиции прайс-листа.
            /// </summary>
            public const string PriceListPostionId = "fnt_price_list_positionid";
            /// <summary>
            /// Логическое имя: Ссылка на прайс-лист.
            /// </summary>
            public const string PriceListReference = "fnt_price_list";
            /// <summary>
            /// Логическое имя: Ссылка на территорию.
            /// </summary>
            public const string TerritoryReference = "fnt_territoryid";
            /// <summary>
            /// Логическое имя: Ссылка на предмет.
            /// </summary>
            public const string SubjectReference = "fnt_subject";
            /// <summary>
            /// Логическое имя: Ссылка на формат подготовки.
            /// </summary>
            public const string FormatPreparationReference = "fnt_format_preparation";
            /// <summary>
            /// Логическое имя: Ссылка на формат проведения.
            /// </summary>
            public const string FormatConductionReference = "fnt_format_condaction";
            /// <summary>
            /// Логическое имя: Цена.
            /// </summary>
            public const string Price = "fnt_price";
        }

        /// <summary>
        /// Логическое имя сущности позиции прайс-листа.
        /// </summary>
        public const string EntityLogicalName = "fnt_price_list_position";

        /// <summary>
        /// Получает или задает идентификатор позиции прайс-листа.
        /// </summary>
        public Guid PriceListPostionId
        {
            get { return GetAttributeValue<Guid>(Metadata.PriceListPostionId); }
            set { Attributes[Metadata.PriceListPostionId] = value; }
        }

        /// <summary>
        /// Получает или задает ссылку на прайс-лист.
        /// </summary>
        public EntityReference PriceListReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.PriceListReference); }
            set { Attributes[Metadata.PriceListReference] = value; }
        }

        /// <summary>
        /// Получает или задает ссылку на территорию.
        /// </summary>
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }

        /// <summary>
        /// Получает или задает ссылку на предмет.
        /// </summary>
        public EntityReference SubjectReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.SubjectReference); }
            set { Attributes[Metadata.SubjectReference] = value; }
        }

        /// <summary>
        /// Получает или задает ссылку на формат подготовки.
        /// </summary>
        public EntityReference FormatPreparationReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.FormatPreparationReference); }
            set { Attributes[Metadata.FormatPreparationReference] = value; }
        }

        /// <summary>
        /// Получает или задает ссылку на формат проведения.
        /// </summary>
        public EntityReference FormatConductionReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.FormatConductionReference); }
            set { Attributes[Metadata.FormatConductionReference] = value; }
        }

        /// <summary>
        /// Получает или задает цену.
        /// </summary>
        public Money Price
        {
            get { return GetAttributeValue<Money>(Metadata.Price); }
            set { Attributes[Metadata.Price] = value; }
        }
    }
}



