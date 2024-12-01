using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс, представляющий сущность "Interest" в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class Interest : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Interest"/>.
        /// </summary>
        public Interest() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для атрибутов сущности "Interest".
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: почта интереса.
            /// </summary>
            public const string Email = "fnt_email";
            /// <summary>
            /// Логическое имя: ProductCartId идентификатор интереса.
            /// </summary>
            public const string InterestId = "fnt_interestid";
            /// <summary>
            /// Логическое имя: имени интереса.
            /// </summary>
            public const string FirstName = "fnt_first_name";
            /// <summary>
            /// Логическое имя: ссылки контакта интереса.
            /// </summary>
            public const string ContactReference = "fnt_kontid";
            /// <summary>
            /// Логическое имя: отчества интереса.
            /// </summary>
            public const string MiddleName = "fnt_middle_name";
            /// <summary>
            /// Логическое имя: статуса интереса.
            /// </summary>
            public const string Status = "fnt_status";
            /// <summary>
            /// Логическое имя: телефон интереса.
            /// </summary>
            public const string Phone = "fnt_phone";
            /// <summary>
            /// Логическое имя: ссылки территории интереса.
            /// </summary>
            public const string TerritoryReference = "fnt_terrid";
            /// <summary>
            /// Логическое имя: фамилия интереса.
            /// </summary>
            public const string LastName = "fnt_last_name";
            /// <summary>
            /// Логическое имя: ответственный интереса.
            /// </summary>
            public const string OwnerId = "ownerid";
        }

        /// <summary>
        /// Перечисление: представляющее возможные статусы интереса.
        /// </summary>
        public enum InterestStepStatus
        {
            New = 797_720_001,
            InProgress = 797_720_002,
            Agreement = 797_720_003,
            Refusal = 797_720_004
        }
        /// <summary>Логическое имя: таблицы интереса </summary>  
        public const string EntityLogicalName = "fnt_interest";

        /// <summary>
        /// Свойство: Получает или задает адрес электронной почты.
        /// </summary>
        public string Email
        {
            get { return GetAttributeValue<string>(Metadata.Email); }
            set { Attributes[Metadata.Email] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает идентификатор интереса.
        /// </summary>
        public Guid InterestId
        {
            get { return GetAttributeValue<Guid>(Metadata.InterestId); }
            set { Attributes[Metadata.InterestId] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает имя.
        /// </summary>
        public string FirstName
        {
            get { return GetAttributeValue<string>(Metadata.FirstName); }
            set { Attributes[Metadata.FirstName] = value; }
        }

        /// <summary>
        ///Свойство: Получает или задает отчество.
        /// </summary>
        public string MiddleName
        {
            get { return GetAttributeValue<string>(Metadata.MiddleName); }
            set { Attributes[Metadata.MiddleName] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает статус.
        /// </summary>
        public OptionSetValue Status
        {
            get { return GetAttributeValue<OptionSetValue>(Metadata.Status); }
            set { Attributes[Metadata.Status] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает номер телефона.
        /// </summary>
        public string Phone
        {
            get { return GetAttributeValue<string>(Metadata.Phone); }
            set { Attributes[Metadata.Phone] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает ссылку на контакт.
        /// </summary>
        public EntityReference ContactReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.ContactReference); }
            set { Attributes[Metadata.ContactReference] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает ссылку на территорию.
        /// </summary>
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }

        /// <summary>
        /// Свойство: Получает или задает фамилию.
        /// </summary>
        public string LastName
        {
            get { return GetAttributeValue<string>(Metadata.LastName); }
            set { Attributes[Metadata.LastName] = value; }
        }

        /// <summary>
        /// Свойство: Получает статус в виде перечисления InterestStepStatus.
        /// </summary>
        public InterestStepStatus StatusToEnum => (InterestStepStatus)Status?.Value;

        /// <summary>
        /// Свойство: Получает или задает ссылку на владельца интереса.
        /// </summary>
        public EntityReference InterestOwnerId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.OwnerId); }
            set { Attributes[Metadata.OwnerId] = value; }
        }
    }
}




