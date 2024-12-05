using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Представляет значение переменной окружения в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class EnvironmentVariableValue : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EnvironmentVariableValue"/>.
        /// </summary>
        public EnvironmentVariableValue() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="EnvironmentVariableValue"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Идентификатор определения переменной окружения.
            /// </summary>
            public const string EnvironmentVariableDefinitionId = "environmentvariabledefinitionid";
            /// <summary>
            /// Логическое имя: Значение.
            /// </summary>
            public const string Value = "value";
        }

        /// <summary>
        /// Логическое имя сущности значения переменной окружения.
        /// </summary>
        public const string EntityLogicalName = "environmentvariablevalue";

        /// <summary>
        /// Получает или задает идентификатор определения переменной окружения.
        /// </summary>
        public EntityReference EnvironmentVariableDefinitionId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.EnvironmentVariableDefinitionId); }
            set { Attributes[Metadata.EnvironmentVariableDefinitionId] = value; }
        }

        /// <summary>
        /// Получает или задает значение переменной окружения.
        /// </summary>
        public string Value
        {
            get { return GetAttributeValue<string>(Metadata.Value); }
            set { Attributes[Metadata.Value] = value; }
        }
    }
}



