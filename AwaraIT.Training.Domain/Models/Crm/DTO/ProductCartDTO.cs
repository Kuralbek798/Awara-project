using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.DTO
{
    /// <summary>
    /// Класс <c>ProductCart</c> представляет сущность продуктовой корзины в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class ProductCartDTO : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Territory"/>.
        /// </summary>
        public ProductCartDTO() : base(EntityLogicalName) { }

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
            /// Логическое имя: ссылки продукта.
            /// </summary>
            public const string ProductReference = "fnt_productid";
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

            /// <summary>
            /// Псевдоним объекта: сущности "возможная сделка".
            /// </summary>
            public const string PosibleDealEntityAlias = "fnt_possibleDeal"; // Alias for possible deal entity 
        }
        /// <summary>
        /// Логическое имя сущности корзины.
        /// </summary>
        public const string EntityLogicalName = "fnt_product_cart";



        /// <summary>
        /// Свойство: ProductCartId продуктовой корзины.
        /// </summary>
        public Guid ProductCartId
        {
            get { return GetAttributeValue<Guid>(Metadata.ProductCartId); }
            set { Attributes[Metadata.ProductCartId] = value; }
        }
        /// <summary>
        /// Свойство: ссылка продукта.
        /// </summary>
        public string ProductReference
        {
            get { return GetAttributeValue<string>(Metadata.ProductReference); }
            set { Attributes[Metadata.ProductReference] = value; }
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
        /// <summary>
        /// Свойство: Id возможной сделки полученое по "алиас".
        /// </summary>
        public Guid? PossibleDealId
        {
            get
            {
                var aliasedValue = GetAttributeValue<AliasedValue>(Metadata.PosibleDealEntityAlias);
                return aliasedValue?.Value as Guid?;
            }
            set
            {
                Attributes[Metadata.PosibleDealEntityAlias] = value;
            }
        }
    }
}
