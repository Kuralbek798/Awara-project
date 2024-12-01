using AwaraIT.Training.Domain.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    /// <summary>
    /// Класс <c>Log</c> представляет сущность журнала в CRM.
    /// </summary>
    [EntityLogicalName(EntityLogicalName)]
    public class Log : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Log"/>.
        /// </summary>
        public Log() : base(EntityLogicalName) { }

        /// <summary>
        /// Содержит метаданные для полей сущности <see cref="Log"/>.
        /// </summary>
        public static class Metadata
        {
            /// <summary>
            /// Уровень журнала.
            /// </summary>
            public const string Level = "awr_level";

            /// <summary>
            /// Описание.
            /// </summary>
            public const string Description = "awr_description";

            /// <summary>
            /// Тема.
            /// </summary>
            public const string Subject = "awr_subject";

            /// <summary>
            /// Тип сущности.
            /// </summary>
            public const string EntityType = "awr_entitytype";

            /// <summary>
            /// Идентификатор сущности.
            /// </summary>
            public const string EntityId = "awr_entityid";

            /// <summary>
            /// Перечисление уровней журнала.
            /// </summary>
            public enum LevelOptions
            {
                /// <summary>
                /// Трассировка.
                /// </summary>
                TRACE = 752_440_000,

                /// <summary>
                /// Отладка.
                /// </summary>
                DEBUG = 752_440_001,

                /// <summary>
                /// Информация.
                /// </summary>
                INFO = 752_440_002,

                /// <summary>
                /// Предупреждение.
                /// </summary>
                WARNING = 752_440_003,

                /// <summary>
                /// Ошибка.
                /// </summary>
                ERROR = 752_440_004,

                /// <summary>
                /// Критическая ошибка.
                /// </summary>
                CRITICAL = 752_440_005
            }
        }

        /// <summary>
        /// Логическое имя сущности журнала.
        /// </summary>
        public const string EntityLogicalName = "awr_log";

        /// <summary>
        /// Уровень журнала.
        /// </summary>
        public Metadata.LevelOptions? Level
        {
            get { return (Metadata.LevelOptions?)GetAttributeValue<OptionSetValue>(Metadata.Level)?.Value; }
            set { Attributes[Metadata.Level] = value != null ? new OptionSetValue((int)value.Value) : null; }
        }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description
        {
            get { return GetAttributeValue<string>(Metadata.Description); }
            set { Attributes[Metadata.Description] = value?.Crop(10000); }
        }

        /// <summary>
        /// Тема.
        /// </summary>
        public string Subject
        {
            get { return GetAttributeValue<string>(Metadata.Subject); }
            set { Attributes[Metadata.Subject] = value?.Crop(1000); }
        }

        /// <summary>
        /// Тип сущности.
        /// </summary>
        public string EntityType
        {
            get { return GetAttributeValue<string>(Metadata.EntityType); }
            set { Attributes[Metadata.EntityType] = value; }
        }

        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        public string EntityId
        {
            get { return GetAttributeValue<string>(Metadata.EntityId); }
            set { Attributes[Metadata.EntityId] = value; }
        }
    }
}


