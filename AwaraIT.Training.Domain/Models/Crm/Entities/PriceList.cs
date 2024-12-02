using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс <c>Price</c> представляет сущность прайс-листа в CRM.
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
        }

        /// <summary>
        /// Логическое имя сущности прайс-листа.
        /// </summary>
        public const string EntityLogicalName = "fnt_price_list";

        /// <summary>
        /// Получает или задает имя прайс-листа.
        /// </summary>
        public Guid PriceListName
        {
            get { return GetAttributeValue<Guid>(Metadata.PriceListName); }
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
    }
}









