using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces;
using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using AwaraIT.Training.Domain.Extensions;
using AwaraIT.Kuralbek.Plugins.Hellpers;
using System.ComponentModel.Design;
using AwaraIT.Kuralbek.Plugins.Helpers;

namespace AwaraIT.Kuralbek.Plugins.InteresPlugin
{
    public class IntetestPluginAssignmentOnCreation : PluginBase
    {
        private readonly string _teamName = "fnt___Колл-центр";
        private Logger _log;

        public IntetestPluginAssignmentOnCreation()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(Interest.EntityLogicalName)
                .When(PluginStage.PreOperation)
                .Execute(Execute);
        }

        /// <summary>
        /// Основной метод выполнения плагина, который назначает владельца интереса наименее загруженному пользователю.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <exception cref="InvalidPluginExecutionException">Выбрасывается при возникновении ошибки во время выполнения плагина.</exception>
        private void Execute(IContextWrapper wrapper)
        {
            _log = new Logger(wrapper.Service);
            try
            {
                var interest = wrapper.TargetEntity.ToEntity<Interest>();

                if (interest != null && interest.Status != null)
                {
                    if (interest.StatusToEnum == InterestStepStatus.New)
                    {
                        var contact = FindOrCreateContact(wrapper, interest);
                        interest.ContactReference = contact.ToEntityReference();

                        var usersIdList = GetUserIdListByTeamName(wrapper.Service);
                        _log.INFO($"Получены пользователи команды, количество: {usersIdList.Count}");

                        // Условия для поиска записей 
                        var conditionsExpressions = PluginHelper.SetConditionsExpressions(usersIdList, InterestStepStatus.InProgress.ToIntValue(), InterestStepStatus.New.ToIntValue());
                        // Получаем наименее загруженного пользователя для сущности Interest
                        var responsibleUser = PluginHelper.GetLeastLoadedEntity(wrapper, conditionsExpressions, Interest.EntityLogicalName);

                        if (responsibleUser.Id == Guid.Empty)
                        {
                            return;
                        }

                        interest.OwnerId = responsibleUser.ToEntityReference();

                        _log.INFO($"InterestAssignmentPlugin: Владелец интереса назначен - " +
                        $"ID интереса: {interest.Id}, ID владельца: {interest.OwnerId.Id} имя {interest.OwnerId.Name} логическое имя {interest.OwnerId.LogicalName}");
                    }
                }
                else
                {
                    _log.ERROR($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)}, interest Entity is null in targetEntity");
                    throw new ArgumentNullException($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)}, interest Entity is null in targetEntity");
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(Execute)} of {nameof(IntetestPluginAssignmentOnCreation)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(Execute)} method of {nameof(IntetestPluginAssignmentOnCreation)}.", ex);
            }
        }

        /// <summary>
        /// Находит или создает контакт на основе предоставленной информации об интересе.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <param name="interest">Информация об интересе.</param>
        /// <returns>Сущность контакта.</returns>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время поиска или создания контакта.</exception>
        private Entity FindOrCreateContact(IContextWrapper wrapper, Interest interest)
        {
            try
            {
                string email = interest.Email;
                string phone = interest.Phone;

                // Поиск контакта
                var query = new QueryExpression(Contact.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Contact.Metadata.ContactId)
                };
                query.Criteria.AddCondition(Contact.Metadata.Email, ConditionOperator.Equal, email);
                query.Criteria.AddCondition(Contact.Metadata.Phone, ConditionOperator.Equal, phone);

                var contacts = wrapper.Service.RetrieveMultiple(query).Entities;
                if (contacts.Any())
                {
                    var contact = contacts.First();
                    _log.INFO($"Контакт назначен к интересу {contact.Id}");
                    return contact;
                }
                else
                {
                    // Создание нового контакта
                    var contact = new Entity(Contact.EntityLogicalName)
                    {
                        [Contact.Metadata.Email] = email,
                        [Contact.Metadata.FirstName] = interest.FirstName,
                        [Contact.Metadata.MiddleName] = interest.MiddleName,
                        [Contact.Metadata.Phone] = phone,
                        [Contact.Metadata.TerritoryReference] = interest.TerritoryReference,
                        [Contact.Metadata.LastName] = interest.LastName
                    };

                    contact.Id = wrapper.Service.Create(contact);

                    return contact;
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(FindOrCreateContact)} of {nameof(IntetestPluginAssignmentOnCreation)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(FindOrCreateContact)} method of {nameof(IntetestPluginAssignmentOnCreation)}.", ex);
            }
        }

        /// <summary>
        /// Получает список идентификаторов пользователей, которые принадлежат определенной команде.
        /// </summary>
        /// <param name="service">Экземпляр IOrganizationService, используемый для выполнения запроса.</param>
        /// <returns>Список GUID, представляющих идентификаторы пользователей, которые принадлежат указанной команде.</returns>
        /// <exception cref="Exception">Выбрасывается, когда происходит ошибка во время выполнения запроса.</exception>
        private List<Guid> GetUserIdListByTeamName(IOrganizationService service)
        {
            try
            {
                // Создаем запрос для получения пользователей, связанных с указанной командой
                var userQuery = new QueryExpression(User.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(User.Metadata.SystemUserId), // Указываем столбцы для получения
                    LinkEntities =
                    {
                        new LinkEntity(User.EntityLogicalName, TeammembershipNN.EntityLogicalName, User.Metadata.SystemUserId, TeammembershipNN.Metadata.SystemUserId, JoinOperator.Inner)
                        {
                            LinkEntities =
                            {
                                new LinkEntity(TeammembershipNN.EntityLogicalName, Team.EntityLogicalName, TeammembershipNN.Metadata.TeamId, Team.Metadata.TeamId, JoinOperator.Inner)
                                {
                                    LinkCriteria = new FilterExpression
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression(Team.Metadata.Name, ConditionOperator.Equal, _teamName) // Фильтрация по имени команды
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                // Выполняем запрос и получаем сущности пользователей
                var userEntities = service.RetrieveMultiple(userQuery).Entities;

                // Извлекаем идентификаторы пользователей из полученных сущностей
                var userIds = userEntities.Select(u => u.GetAttributeValue<Guid>(User.Metadata.SystemUserId)).ToList();

                return userIds;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Error in method {nameof(GetUserIdListByTeamName)} of {nameof(IntetestPluginAssignmentOnCreation)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(GetUserIdListByTeamName)} method of {nameof(IntetestPluginAssignmentOnCreation)}.", ex);
            }
        }
    }
}

