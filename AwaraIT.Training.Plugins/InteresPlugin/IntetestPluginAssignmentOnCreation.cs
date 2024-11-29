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
                    _log.ERROR($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)},interest Entity is null in targetEntity");
                    throw new ArgumentNullException($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)}, interest Entity is null in targetEntity");
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(Execute)} {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"Ошибка в {nameof(Execute)}: {ex.Message}", ex);
            }

        }

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
                _log.ERROR($"Ошибка в {nameof(FindOrCreateContact)} {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(FindOrCreateContact)}: {ex.Message}", ex);
            }
        }

        private List<Guid> GetUserIdListByTeamName(IOrganizationService service)
        {
            try
            {

                var userQuery = new QueryExpression(User.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(User.Metadata.SystemUserId),
                    LinkEntities =
                    {
                       new LinkEntity(User.EntityLogicalName, Teammembership.EntityLogicalName, "systemuserid", "systemuserid", JoinOperator.Inner)
                       {
                             LinkEntities =
                             {
                                   new LinkEntity(Teammembership.EntityLogicalName, WorkGroup.EntityLogicalName, "teamid", "teamid", JoinOperator.Inner)
                                   {
                                       LinkCriteria = new FilterExpression
                                       {
                                              Conditions =
                                              {
                                                new ConditionExpression(WorkGroup.Metadata.Name, ConditionOperator.Equal, _teamName)
                                               }
                                        }
                                   }
                             }
                       }
                    }
                };

                // Execute the query and get the list of users
                var userEntities = service.RetrieveMultiple(userQuery).Entities;

                var userIds = userEntities.Select(u => u.GetAttributeValue<Guid>(User.Metadata.SystemUserId)).ToList();

                return userIds;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в методе {nameof(GetUserIdListByTeamName)}: {ex.Message},  {ex}");
                throw new Exception($"Ошибка в {nameof(GetUserIdListByTeamName)}: {ex.Message}", ex);
            }
        }
    }
}
