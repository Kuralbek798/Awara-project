
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using System.Runtime.CompilerServices;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс <c>PosibleDeal</c> представляет сущность возможной сделки в CRM.
    /// </summary>
   // [DataContract(Namespace = "AwaraIT.Training.Domain.Models.Crm.Entities")]
    [EntityLogicalName(EntityLogicalName)]
    public class PossibleDeal : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PossibleDeal"/>.
        /// </summary>
        public PossibleDeal() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="PossibleDeal"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: идентификатор возможной сделки.
            /// </summary>
            public const string PosibleDealId = "fnt_posible_dealid";

            /// <summary>
            /// Логическое имя: ссылка на контакт.
            /// </summary>
            public const string ContactReference = "fnt_kontactid";

            /// <summary>
            /// Логическое имя: статус возможной сделки.
            /// </summary>
            public const string Status = "fnt_status";

            /// <summary>
            /// Логическое имя:  цена.
            /// </summary>
            public const string Price = "fnt_price";

            /// <summary>
            /// Логическое имя:  скидка.
            /// </summary>
            public const string Discount = "fnt_discount";

            /// <summary>
            /// Логическое имя:  цена после скидки.
            /// </summary>
            public const string PriceAfterDiscount = "fnt_price_after_discount";

            /// <summary>
            /// Логическое имя: ссылка на территорию.
            /// </summary>
            public const string TerritoryReference = "fnt_territoryid";
        }

        /// <summary>
        /// Перечисление статусов возможной сделки.
        /// </summary>
        public enum PossibleDealStepStatus
        {
            /// <summary>
            /// Открыта.
            /// </summary>
            Open = 797_720_000,

            /// <summary>
            /// В работе.
            /// </summary>
            InProgress = 797_720_001,

            /// <summary>
            /// Выиграна.
            /// </summary>
            Won = 797_720_002,
        }

        /// <summary>
        /// Логическое имя: сущности "возможная сделка".
        /// </summary>
        public const string EntityLogicalName = "fnt_posible_deal";
        /// <summary>
        /// Псевдоним объекта: сущности "возможная сделка".
        /// </summary>
        public const string EntityAlias = "fnt_possibleDeal";

        /// <summary>
        /// Свойство: Идентификатор возможной сделки.
        /// </summary>
        [DataMember]
        public Guid PosibleDealId
        {
            get { return GetAttributeValue<Guid>(Metadata.PosibleDealId); }
            set { Attributes[Metadata.PosibleDealId] = value; }
        }

        /// <summary>
        /// Свойство: Ссылка на контакт.
        /// </summary>
        [DataMember]
        public EntityReference ContactReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.ContactReference); }
            set { Attributes[Metadata.ContactReference] = value; }
        }

        /// <summary>
        /// Свойство: Статус возможной сделки.
        /// </summary>
        [DataMember]
        public OptionSetValue Status
        {
            get { return GetAttributeValue<OptionSetValue>(Metadata.Status); }
            set { Attributes[Metadata.Status] = value; }
        }

        /// <summary>
        /// Свойство: цена.
        /// </summary>
        [DataMember]
        public decimal BasePriceSum
        {
            get { return GetAttributeValue<decimal>(Metadata.Price); }
            set { Attributes[Metadata.Price] = value; }
        }

        /// <summary>
        /// Свойство: скидка.
        /// </summary>
        [DataMember]
        public decimal DiscountSum
        {
            get { return GetAttributeValue<decimal>(Metadata.Discount); }
            set { Attributes[Metadata.Discount] = value; }
        }

        /// <summary>
        /// Свойство: Сумма цен после скидки.
        /// </summary>
        [DataMember]
        public decimal PriceAfterDiscountSum
        {
            get { return GetAttributeValue<decimal>(Metadata.PriceAfterDiscount); }
            set { Attributes[Metadata.PriceAfterDiscount] = value; }
        }

        /// <summary>
        /// Свойство: Ссылка на территорию.
        /// </summary>
        [DataMember]
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }

        /// <summary>
        /// Свойство: Статус возможной сделки в виде перечисления.
        /// </summary>
        public PossibleDealStepStatus StatusEnum
        {
            get { return (PossibleDealStepStatus)Status?.Value; }
        }
    }
}


