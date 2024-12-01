using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{


    [EntityLogicalName(EntityLogicalName)]
    public class Product : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Product"/>.
        /// </summary>
        public Product() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="Product"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Формат проведения.
            /// </summary>
            public const string FormatConductionReference = "fnt_event_typeid";
            /// <summary>
            /// Логическое имя: Формат подготовки.
            /// </summary>
            public const string FormatPreparationReference = "fnt_education_typeid";
            /// <summary>
            /// Логическое имя: предмета.
            /// </summary>
            public const string SubjectPreparationReference = "fnt_subject";
            /// <summary>
            /// Логическое имя: идентификатора продукта.
            /// </summary>
            public const string ProductId = "fnt_education_productid";
        }
        /// <summary>
        /// Свойство: идентификатор продукта.
        /// </summary>
        public Guid ProductId
        {
            get { return GetAttributeValue<Guid>(Metadata.ProductId); }
            set { Attributes[Metadata.ProductId] = value; }
        }
        /// <summary>
        /// Логическое имя сущности продукта.
        /// </summary>
        public const string EntityLogicalName = "fnt_education_product";

        /// <summary>
        /// Свойство:  Формата проведения.
        /// </summary>
        public Guid EventTypeReference
        {
            get { return GetAttributeValue<Guid>(Metadata.FormatConductionReference); }
            set { Attributes[Metadata.FormatConductionReference] = value; }
        }
        /// <summary>
        /// Свойство:  Формата подготовки.
        /// </summary>
        public Guid EducationTypeReference
        {
            get { return GetAttributeValue<Guid>(Metadata.FormatPreparationReference); }
            set { Attributes[Metadata.FormatPreparationReference] = value; }
        }
        /// <summary>
        /// Свойство: предмет.
        /// </summary>
        public Guid SubjectReference
        {
            get { return GetAttributeValue<Guid>(Metadata.SubjectPreparationReference); }
            set { Attributes[Metadata.SubjectPreparationReference] = value; }
        }

    }
}
