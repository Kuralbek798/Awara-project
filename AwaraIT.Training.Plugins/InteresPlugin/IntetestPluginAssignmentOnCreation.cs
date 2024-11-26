﻿using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
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

                if (interest == null)
                {
                    // _log.ERROR($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)},interest Entity is null in targetEntity");
                    throw new ArgumentNullException($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)}, interest Entity is null in targetEntity");
                }

                var contact = FindOrCreateContact(wrapper, interest);

                interest.ContactReference = contact.ToEntityReference();
                // _log.INFO($"InterestAssignmentPlugin: Контакт назначен - " +
                //$"Интерес [LogicalName: {wrapper.TargetEntity.LogicalName}, ID: {wrapper.TargetEntity.Id}]; " +
                //$"Контакт [LogicalName: {contact.LogicalName}, ID: {contact.Id}]");

                var responsibleUser = GetLeastLoadedUser(wrapper, interest.TerritoryReference.Id);
                interest.OwnerId = responsibleUser.ToEntityReference();

                // _log.INFO($"InterestAssignmentPlugin: Владелец интереса назначен - " +
                //  $"ID интереса: {interest.Id}, ID владельца: {interest.OwnerId.Id}");
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(Execute)} {ex.Message}, {ex}");
                //Trace("InterestAssignmentPlugin обнаружила ошибку: {0}", ex.ToString());
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
                    ColumnSet = new ColumnSet(true)
                };
                query.Criteria.AddCondition(Contact.Metadata.Email, ConditionOperator.Equal, email);
                query.Criteria.AddCondition(Contact.Metadata.Phone, ConditionOperator.Equal, phone);

                var contacts = wrapper.Service.RetrieveMultiple(query).Entities;
                if (contacts.Any())
                {
                    var contact = contacts.First();
                    // _log.INFO($"Контакт назначен к интересу {contact.Id}");
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
                    //_log.INFO($"Контакт создан и назначен к интересу {contact.Id}");
                    return contact;
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(FindOrCreateContact)} {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(FindOrCreateContact)}: {ex.Message}", ex);
            }
        }

        private Entity GetLeastLoadedUser(IContextWrapper wrapper, Guid territoryId)
        {
            try
            {
                Guid teamId = GetTeamId(wrapper.Service, _teamName);
                // _log.INFO($"Получен ID команды: {teamId}");

                var users = GetUserIdListInTeam(wrapper.Service, teamId);
                // _log.INFO($"Получены пользователи команды, количество: {users.Count}");

                var loadQuery = new QueryExpression(Interest.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Interest.Metadata.InterestId, Interest.Metadata.OwnerId),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                    {
                        new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, users.ToArray()),
                        new ConditionExpression(EntityCommon.StatusCode, ConditionOperator.Equal, InterestStepStatus.InProgress.ToIntValue()),
                        new ConditionExpression(Interest.Metadata.TerritoryReference, ConditionOperator.Equal, territoryId)
                    }
                    }
                };
                var interestRecords = wrapper.Service.RetrieveMultiple(loadQuery).Entities;

                var userLoadCounts = users.ToDictionary(userId => userId, userId => 0);
                foreach (var record in interestRecords)
                {
                    var userId = record.ToEntity<Interest>().OwnerId.Id;
                    if (userLoadCounts.ContainsKey(userId))
                    {
                        userLoadCounts[userId]++;
                    }
                }

                var leastLoadedUserId = userLoadCounts.OrderBy(entry => entry.Value).FirstOrDefault().Key;
                return new Entity(User.EntityLogicalName, leastLoadedUserId);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(GetLeastLoadedUser)} {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(GetLeastLoadedUser)}: {ex.Message}", ex);
            }
        }
        private Guid GetTeamId(IOrganizationService service, string teamName)
        {
            try
            {
                var teamQuery = new QueryExpression(WorkGroup.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(WorkGroup.Metadata.TeamId)
                };
                teamQuery.Criteria.AddCondition(WorkGroup.Metadata.Name, ConditionOperator.Equal, teamName);

                var team = service.RetrieveMultiple(teamQuery).Entities
                    .Select(t => t.ToEntity<WorkGroup>())
                    .FirstOrDefault();
                if (team == null)
                {
                    _log.ERROR($"Ошибка в {nameof(GetTeamId)}: команда не найдена, teamName: {teamName}");
                    throw new ArgumentNullException($"Ошибка в {nameof(GetTeamId)}: команда не найдена, teamName: {teamName}");
                }

                return team.TeamId;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в методе {nameof(GetTeamId)}: {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(GetTeamId)}: {ex.Message}", ex);
            }
        }
        private List<Guid> GetUserIdListInTeam(IOrganizationService service, Guid teamId)
        {
            try
            {
                var membershipQuery = new QueryExpression(Teammembership.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Teammembership.Metadata.SystemUserId),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(Teammembership.Metadata.TeamId, ConditionOperator.Equal, teamId)
                        }
                    }
                };

                return service.RetrieveMultiple(membershipQuery).Entities
                    .Select(m => m.ToEntity<Teammembership>().SystemUserId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в методе {nameof(GetUserIdListInTeam)}: {ex.Message},  {ex}");
                throw new Exception($"Ошибка в {nameof(GetUserIdListInTeam)}: {ex.Message}", ex);
            }
        }
    }
}
