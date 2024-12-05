using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Представляет определение переменной окружения в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class EnvironmentVariableDefinition : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EnvironmentVariableDefinition"/>.
        /// </summary>
        public EnvironmentVariableDefinition() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="EnvironmentVariableDefinition"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Логическое имя: Идентификатор определения переменной окружения.
            /// </summary>
            public const string EnvironmentVariableDefinitionId = "environmentvariabledefinitionid";
            /// <summary>
            /// Логическое имя: Значение по умолчанию.
            /// </summary>
            public const string DefaultValue = "defaultvalue";
            /// <summary>
            /// Логическое имя: Имя схемы.
            /// </summary>
            public const string SchemaName = "schemaname";
        }

        /// <summary>
        /// Логическое имя сущности определения переменной окружения.
        /// </summary>
        public const string EntityLogicalName = "environmentvariabledefinition";

        /// <summary>
        /// Получает или задает значение по умолчанию.
        /// </summary>
        public string DefaultValue
        {
            get { return GetAttributeValue<string>(Metadata.DefaultValue); }
            set { Attributes[Metadata.DefaultValue] = value; }
        }

        /// <summary>
        /// Получает или задает имя схемы.
        /// </summary>
        public string SchemaName
        {
            get { return GetAttributeValue<string>(Metadata.SchemaName); }
            set { Attributes[Metadata.SchemaName] = value; }
        }
    }
}




