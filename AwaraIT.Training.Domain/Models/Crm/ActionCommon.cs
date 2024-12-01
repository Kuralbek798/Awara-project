namespace AwaraIT.Training.Domain.Models.Crm
{
    /// <summary>
    /// Класс <c>ActionCommon</c> содержит строковые константы, представляющие общие имена полей, используемые в CRM-действиях.
    /// Эти константы можно использовать, чтобы избежать жесткого кодирования имен полей по всему коду, что делает код более поддерживаемым и менее подверженным ошибкам.
    /// </summary>
    public class ActionCommon
    {
        /// <summary>
        /// Отправители.
        /// </summary>
        public const string From = "from";

        /// <summary>
        /// Получатели.
        /// </summary>
        public const string To = "to";

        /// <summary>
        /// Копия.
        /// </summary>
        public const string Cc = "cc";

        /// <summary>
        /// Скрытая копия.
        /// </summary>
        public const string Bcc = "bcc";

        /// <summary>
        /// Тема.
        /// </summary>
        public const string Subject = "subject";

        /// <summary>
        /// Описание.
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// Запланированное время начала.
        /// </summary>
        public const string Sch_start = "scheduledstart";

        /// <summary>
        /// Запланированное время окончания.
        /// </summary>
        public const string Sch_end = "scheduledend";

        /// <summary>
        /// Время отправки.
        /// </summary>
        public const string SendOn = "senton";

        /// <summary>
        /// Фактическое время начала.
        /// </summary>
        public const string ActualStart = "actualstart";

        /// <summary>
        /// Фактическое время окончания.
        /// </summary>
        public const string ActualEnd = "actualend";

        /// <summary>
        /// Идентификатор действия.
        /// </summary>
        public const string ActivityId = "activityid";

        /// <summary>
        /// Объект, к которому относится запись.
        /// </summary>
        public const string RegardingObjectId = "regardingobjectid";

        /// <summary>
        /// Имя объекта, к которому относится запись.
        /// </summary>
        public const string RegardingObjectIdName = "regardingobjectidname";

        /// <summary>
        /// Идентификатор участника.
        /// </summary>
        public const string PartyId = "partyid";
    }
}

