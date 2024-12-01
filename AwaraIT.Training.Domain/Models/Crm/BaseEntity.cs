
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Serialization;

namespace AwaraIT.Training.Domain.Models.Crm
{

    /// <summary>
    /// Базовый класс для всех CRM-сущностей, содержащий общие свойства.
    /// </summary>
    [DataContract]
    public class BaseEntity : Entity
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BaseEntity"/> с указанным именем CRM-сущности.
        /// </summary>
        /// <param name="crmEntityName">Имя CRM-сущности.</param>
        public BaseEntity(string crmEntityName) : base(crmEntityName) { }

        /// <summary>
        /// Дата и время создания записи.
        /// </summary>
        public DateTime CreatedOn
        {
            get { return GetAttributeValue<DateTime>(EntityCommon.CreatedOn); }
            set { Attributes[EntityCommon.CreatedOn] = value; }
        }

        /// <summary>
        /// Пользователь, создавший запись.
        /// </summary>
        public EntityReference CreatedBy => GetAttributeValue<EntityReference>(EntityCommon.CreatedBy);

        /// <summary>
        /// Дата и время последнего изменения записи.
        /// </summary>
        public DateTime ModifiedOn
        {
            get { return GetAttributeValue<DateTime>(EntityCommon.ModifiedOn); }
            set { Attributes[EntityCommon.ModifiedOn] = value; }
        }

        /// <summary>
        /// Пользователь, последним изменивший запись.
        /// </summary>
        public EntityReference ModifiedBy => GetAttributeValue<EntityReference>(EntityCommon.ModifiedBy);

        /// <summary>
        /// Владелец записи.
        /// </summary>
        public EntityReference OwnerId
        {
            get { return GetAttributeValue<EntityReference>(EntityCommon.OwnerId); }
            set { Attributes[EntityCommon.OwnerId] = value; }
        }
    }

}
