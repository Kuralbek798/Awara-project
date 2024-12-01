namespace AwaraIT.Training.Domain.Models.Crm
{
   
        /// <summary>
        /// Класс <c>EntityCommon</c> содержит строковые константы, представляющие общие имена полей, используемые в CRM-сущностях.
        /// Эти константы, чтобы избежать жесткого кодирования имен полей. 
        /// </summary>
        public static class EntityCommon
        {
            /// <summary>
            /// Представляет владельца записи.
            /// </summary>
            public const string OwnerId = "ownerid";

            /// <summary>
            /// Представляет состояние записи (например, активное, неактивное).
            /// </summary>
            public const string StateCode = "statecode";

            /// <summary>
            /// Представляет статус записи (например, новая, в процессе, завершена).
            /// </summary>
            public const string StatusCode = "statuscode";

            /// <summary>
            /// Представляет дату и время создания записи.
            /// </summary>
            public const string CreatedOn = "createdon";

            /// <summary>
            /// Представляет пользователя, создавшего запись.
            /// </summary>
            public const string CreatedBy = "createdby";

            /// <summary>
            /// Представляет дату и время последнего изменения записи.
            /// </summary>
            public const string ModifiedOn = "modifiedon";

            /// <summary>
            /// Представляет пользователя, последним изменившего запись.
            /// </summary>
            public const string ModifiedBy = "modifiedby";

            /// <summary>
            /// Представляет ID объекта, к которому относится запись.
            /// </summary>
            public const string RegardingObjectId = "regardingobjectid";
        }  

}
