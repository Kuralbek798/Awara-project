using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.SystemEntities
{
    [EntityLogicalName(EntityLogicalName)]
    public class PossibleDealProductCartNN : BaseEntity
    {
        public PossibleDealProductCartNN() : base(EntityLogicalName) { }

        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: ProductCartId - идентификатор.
            /// </summary>
            public const string PosibleDealProductCartId = "fnt_posible_deal_fnt_product_cartid";

            /// <summary>
            /// Логическое имя: ProductCartId - ссылка продуктовой корзины.
            /// </summary>
            public const string ProductCartId = "fnt_product_cartid";

            /// <summary>
            /// Логическое имя: ProductCartId - ссылка сделки.
            /// </summary>
            public const string PossibleDealId = "fnt_posible_dealid";

        }

        /// <summary>
        /// Логическое имя: таблицы пользователя.
        /// </summary>
        public const string EntityLogicalName = "fnt_posible_deal_fnt_product_cart";

        /// <summary>
        /// Свойство: ProductCartId - идентификатор.
        /// </summary>
        public Guid PosibleDealProductCartId
        {
            get { return GetAttributeValue<Guid>(Metadata.PosibleDealProductCartId); }
            set { Attributes[Metadata.PosibleDealProductCartId] = value; }
        }
        /// <summary>
        /// Свойство: ProductCartId - ссылка продуктовой корзины
        /// </summary>
        public Guid ProductCartId
        {
            get { return GetAttributeValue<Guid>(Metadata.ProductCartId); }
            set { Attributes[Metadata.ProductCartId] = value; }
        }
        /// <summary>
        /// Свойство: ProductCartId - ссылка сделки.
        /// </summary>
        public Guid PossibleDealId
        {
            get { return GetAttributeValue<Guid>(Metadata.PossibleDealId); }
            set { Attributes[Metadata.PossibleDealId] = value; }
        }

    }
}
