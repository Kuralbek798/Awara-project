using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.IdentityModel.Protocols.WSTrust;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс <c>PriceList</c> представляет сущность прайс-листа в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class PriceList : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PriceList"/>.
        /// </summary>
        public PriceList() : base(EntityLogicalName) { }

        /// <summary>
        /// Метаданные для сущности прайс-листа.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Имя прайс-листа.
            /// </summary>
            public const string PriceListName = "fnt_price_list_name";
            /// <summary>
            /// Идентификатор прайс-листа.
            /// </summary>
            public const string PriceListId = "fnt_price_listid";
            /// <summary>
            /// Дата окончания прайс-листа.
            /// </summary>
            public const string PriceListEndDate = "fnt_end_date";
            /// <summary>
            /// Дата создания прайс-листа.
            /// </summary>
            public const string CreatedOn = "createdon";
            /// <summary>
            /// Состояние прайс-листа.
            /// </summary>
            public const string StateCode = "statecode";
        }

        /// <summary>
        /// Логическое имя сущности прайс-листа.
        /// </summary>
        public const string EntityLogicalName = "fnt_price_list";

        /// <summary>
        /// Получает или задает имя прайс-листа.
        /// </summary>
        public string PriceListName
        {
            get { return GetAttributeValue<string>(Metadata.PriceListName); }
            set { Attributes[Metadata.PriceListName] = value; }
        }

        /// <summary>
        /// Получает или задает идентификатор прайс-листа.
        /// </summary>
        public Guid PriceListId
        {
            get { return GetAttributeValue<Guid>(Metadata.PriceListId); }
            set { Attributes[Metadata.PriceListId] = value; }
        }

        /// <summary>
        /// Получает или задает дату окончания прайс-листа.
        /// </summary>
        public DateTime PriceListEndDate
        {
            get { return GetAttributeValue<DateTime>(Metadata.PriceListEndDate); }
            set { Attributes[Metadata.PriceListEndDate] = value; }
        }

        /// <summary>
        /// Получает дату создания прайс-листа.
        /// </summary>
        public DateTime CreatedOn
        {
            get { return GetAttributeValue<DateTime>(Metadata.CreatedOn); }
            set { Attributes[Metadata.CreatedOn] = value; }
        }

        /// <summary>
        /// Получает или задает состояние прайс-листа.
        /// </summary>
        public OptionSetValue StateCode
        {
            get { return GetAttributeValue<OptionSetValue>(Metadata.StateCode); }
            set { Attributes[Metadata.StateCode] = value; }
        }
        /// <summary>
        /// Получает состояние прайс-листа в виде перечисления.
        /// </summary>      

        public StateCodeEnum StatusToEnum => (StateCodeEnum)StateCode?.Value;

        /// <summary>
        /// Перечисление состояний прайс-листа.
        /// </summary>
        public enum StateCodeEnum
        {
            /// <summary>
            /// Активный.
            /// </summary>
            Active = 0,
            /// <summary>
            /// Неактивный.
            /// </summary>
            Inactive = 1
        }


    }
}









