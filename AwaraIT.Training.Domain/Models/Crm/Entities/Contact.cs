using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс <c>Contact</c> представляет сущность контакта в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class Contact : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/>.
        /// </summary>
        public Contact() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="Contact"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Электронная почта.
            /// </summary>
            public const string Email = "fnt_email";

            /// <summary>
            /// Логическое имя: Идентификатор контакта.
            /// </summary>
            public const string ContactId = "fnt_contactid";

            /// <summary>
            /// Логическое имя: Имя.
            /// </summary>
            public const string FirstName = "fnt_first_name";

            /// <summary>
            /// Логическое имя: Отчество.
            /// </summary>
            public const string MiddleName = "fnt_middle_name";

            /// <summary>
            /// Логическое имя: Телефон.
            /// </summary>
            public const string Phone = "fnt_phone_number";

            /// <summary>
            /// Логическое имя: Ссылка на территорию.
            /// </summary>
            public const string TerritoryReference = "fnt_territory";

            /// <summary>
            /// Логическое имя: Фамилия.
            /// </summary>
            public const string LastName = "fnt_last_name";
        }

        /// <summary>
        /// Идентификатор контакта.
        /// </summary>
        public Guid ContactId
        {
            get { return GetAttributeValue<Guid>(Metadata.ContactId); }
            set { Attributes[Metadata.ContactId] = value; }
        }
        /// <summary>
        /// Логическое имя сущности контакта.
        /// </summary>
        public const string EntityLogicalName = "fnt_contact";

        /// <summary>
        /// Свойство: Электронная почта.
        /// </summary>
        public string Email
        {
            get { return GetAttributeValue<string>(Metadata.Email); }
            set { Attributes[Metadata.Email] = value; }
        }

        /// <summary>
        /// Свойство: Имя.
        /// </summary>
        public string FirstName
        {
            get { return GetAttributeValue<string>(Metadata.FirstName); }
            set { Attributes[Metadata.FirstName] = value; }
        }

        /// <summary>
        /// Свойство: Отчество.
        /// </summary>
        public string MiddleName
        {
            get { return GetAttributeValue<string>(Metadata.MiddleName); }
            set { Attributes[Metadata.MiddleName] = value; }
        }

        /// <summary>
        /// Свойство: Телефон.
        /// </summary>
        public string Phone
        {
            get { return GetAttributeValue<string>(Metadata.Phone); }
            set { Attributes[Metadata.Phone] = value; }
        }

        /// <summary>
        /// Свойство: Ссылка на территорию.
        /// </summary>
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }

        /// <summary>
        /// Свойство: Фамилия.
        /// </summary>
        public string LastName
        {
            get { return GetAttributeValue<string>(Metadata.LastName); }
            set { Attributes[Metadata.LastName] = value; }
        }
    }
}



