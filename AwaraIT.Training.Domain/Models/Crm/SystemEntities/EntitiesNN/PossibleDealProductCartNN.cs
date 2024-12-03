/*using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    /// <summary>
    /// Класс <c>PossibleDealProductCartNN</c> представляет связь N:N между возможной сделкой и продуктовой корзиной в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class PossibleDealProductCartNN : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PossibleDealProductCartNN"/>.
        /// </summary>
        public PossibleDealProductCartNN() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="PossibleDealProductCartNN"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: идентификатор связи.
            /// </summary>
            public const string PosibleDealProductCartId = "fnt_posible_deal_fnt_product_cartid";

            /// <summary>
            /// Логическое имя: ссылка на продуктовую корзину.
            /// </summary>
            public const string ProductCartId = "fnt_product_cartid";

            /// <summary>
            /// Логическое имя: ссылка на возможную сделку.
            /// </summary>
            public const string PossibleDealId = "fnt_posible_dealid";
        }

        /// <summary>
        /// Логическое имя сущности связи.
        /// </summary>
        public const string EntityLogicalName = "fnt_posible_deal_fnt_product_cart";

        /// <summary>
        /// Свойство: идентификатор связи.
        /// </summary>
        public Guid PosibleDealProductCartId
        {
            get { return GetAttributeValue<Guid>(Metadata.PosibleDealProductCartId); }
            set { Attributes[Metadata.PosibleDealProductCartId] = value; }
        }

        /// <summary>
        /// Свойство: ссылка на продуктовую корзину.
        /// </summary>
        public Guid ProductCartId
        {
            get { return GetAttributeValue<Guid>(Metadata.ProductCartId); }
            set { Attributes[Metadata.ProductCartId] = value; }
        }

        /// <summary>
        /// Свойство: ссылка на возможную сделку.
        /// </summary>
        public Guid PossibleDealId
        {
            get { return GetAttributeValue<Guid>(Metadata.PossibleDealId); }
            set { Attributes[Metadata.PossibleDealId] = value; }
        }
    }
}

*/