using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /*плагин, который будет получать все продуктовые корзины для 
     * возможной сделки суммировать поля Цена базовая, Скидка,
     * Цена после скидки и заполнять соответствующие поля на возможной сделке
     * Table LogicalName of NN relation:  posible_deal_fnt_product_cart
     */



    /// <summary>
    /// Класс <c>ProductCart</c> представляет сущность продуктовой корзины в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class ProductCart : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Territory"/>.
        /// </summary>
        public ProductCart() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="Territory"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Id продуктовой корзины.
            /// </summary>
            public const string ProductCartId = "fnt_product_cartid";

            /// <summary>
            /// Логическое имя: ссылка продукт.
            /// </summary>
            public const string ProductReference = "fnt_productid";
            /// <summary>
            /// Логическое имя: ссылка возможная сделка.
            /// </summary>
            public const string PossibleDealReference = "fnt_posible_deal";
            /// <summary>
            /// Логическое имя: cкидка.
            /// </summary>
            public const string Discount = "fnt_discount";
            /// <summary>
            /// Логическое имя: цена.
            /// </summary>
            public const string Price = "fnt_price";
            /// <summary>
            /// Логическое имя: цена после скидки.
            /// </summary>
            public const string PriceAfterDiscount = "fnt_price_after_discount";
        }

        /// <summary>
        /// Логическое имя сущности корзина.
        /// </summary>
        public const string EntityLogicalName = "fnt_product_cart";

        /// <summary>
        /// Свойство: идентификатор продуктовой корзины.
        /// </summary>
        public Guid ProductCartId
        {
            get { return GetAttributeValue<Guid>(Metadata.ProductCartId); }
            set { Attributes[Metadata.ProductCartId] = value; }
        }
        /// <summary>
        /// Свойство: ссылка продукта.
        /// </summary>
        public EntityReference ProductReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.ProductReference); }
            set { Attributes[Metadata.ProductReference] = value; }
        }
        /// <summary>
        /// Свойство: ссылка возможная сделка.
        /// </summary>
        public EntityReference PossibleDealReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.PossibleDealReference); }
            set { Attributes[Metadata.PossibleDealReference] = value; }
        }
        /// <summary>
        /// Свойство: cкидка.
        /// </summary>
        public Money Discount
        {
            get { return GetAttributeValue<Money>(Metadata.Discount); }
            set { Attributes[Metadata.Discount] = value; }
        }
        /// <summary>
        /// Свойство: цена.
        /// </summary>
        public Money Price
        {
            get { return GetAttributeValue<Money>(Metadata.Price); }
            set { Attributes[Metadata.Price] = value; }
        }
        /// <summary>
        /// Свойство: цена после скидки.
        /// </summary>
        public Money PriceAfterDiscount
        {
            get { return GetAttributeValue<Money>(Metadata.PriceAfterDiscount); }
            set { Attributes[Metadata.PriceAfterDiscount] = value; }
        }
    }
}
