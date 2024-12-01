using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Serialization;

namespace AwaraIT.Training.Domain.Models.Crm
{

    /// <summary>
    /// Базовый класс для всех CRM-действий, содержащий общие свойства.
    /// </summary>
    [DataContract]
    public class BaseActionEntity : BaseEntity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BaseActionEntity"/> с указанным именем CRM-сущности.
        /// </summary>
        /// <param name="crmEntityName">Имя CRM-сущности.</param>
        public BaseActionEntity(string crmEntityName)
            : base(crmEntityName) { }

        /// <summary>
        /// Идентификатор действия.
        /// </summary>
        public Guid ActivityId
        {
            get { return GetAttributeValue<Guid>(ActionCommon.ActivityId); }
            set { Attributes[ActionCommon.ActivityId] = value; }
        }

        /// <summary>
        /// Отправители.
        /// </summary>
        public Entity[] From
        {
            get { return GetAttributeValue<Entity[]>(ActionCommon.From); }
            set { Attributes[ActionCommon.From] = value; }
        }

        /// <summary>
        /// Получатели.
        /// </summary>
        public Entity[] To
        {
            get { return GetAttributeValue<Entity[]>(ActionCommon.To); }
            set { Attributes[ActionCommon.To] = value; }
        }

        /// <summary>
        /// Копия.
        /// </summary>
        public Entity[] Cc
        {
            get { return GetAttributeValue<Entity[]>(ActionCommon.Cc); }
            set { Attributes[ActionCommon.Cc] = value; }
        }

        /// <summary>
        /// Скрытая копия.
        /// </summary>
        public Entity[] Bcc
        {
            get { return GetAttributeValue<Entity[]>(ActionCommon.Bcc); }
            set { Attributes[ActionCommon.Bcc] = value; }
        }

        /// <summary>
        /// Тема.
        /// </summary>
        public string Subject
        {
            get { return GetAttributeValue<string>(ActionCommon.Subject); }
            set { Attributes[ActionCommon.Subject] = value; }
        }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description
        {
            get { return GetAttributeValue<string>(ActionCommon.Description); }
            set { Attributes[ActionCommon.Description] = value; }
        }

        /// <summary>
        /// Запланированное время начала.
        /// </summary>
        public DateTime? ScheduledStart
        {
            get { return GetAttributeValue<DateTime?>(ActionCommon.Sch_start); }
            set { Attributes[ActionCommon.Sch_start] = value; }
        }

        /// <summary>
        /// Запланированное время окончания.
        /// </summary>
        public DateTime? ScheduledEnd
        {
            get { return GetAttributeValue<DateTime?>(ActionCommon.Sch_end); }
            set { Attributes[ActionCommon.Sch_end] = value; }
        }

        /// <summary>
        /// Время отправки.
        /// </summary>
        public DateTime? SendOn
        {
            get { return GetAttributeValue<DateTime?>(ActionCommon.SendOn); }
            set { Attributes[ActionCommon.SendOn] = value; }
        }

        /// <summary>
        /// Фактическое время начала.
        /// </summary>
        public DateTime? ActualStart
        {
            get { return GetAttributeValue<DateTime?>(ActionCommon.ActualStart); }
            set { Attributes[ActionCommon.ActualStart] = value; }
        }

        /// <summary>
        /// Фактическое время окончания.
        /// </summary>
        public DateTime? ActualEnd
        {
            get { return GetAttributeValue<DateTime?>(ActionCommon.ActualEnd); }
            set { Attributes[ActionCommon.ActualEnd] = value; }
        }

        /// <summary>
        /// Объект, к которому относится запись.
        /// </summary>
        public EntityReference RegardingObjectId
        {
            get { return GetAttributeValue<EntityReference>(ActionCommon.RegardingObjectId); }
            set { Attributes[ActionCommon.RegardingObjectId] = value; }
        }

        /// <summary>
        /// Имя объекта, к которому относится запись.
        /// </summary>
        public string RegardingObjectIdName
        {
            get { return GetAttributeValue<string>(ActionCommon.RegardingObjectIdName); }
            set { Attributes[ActionCommon.RegardingObjectIdName] = value; }
        }
    }
}



