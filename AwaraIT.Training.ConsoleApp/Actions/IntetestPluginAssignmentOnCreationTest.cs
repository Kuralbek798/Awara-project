
using AwaraIT.Training.Application.Core;
using AwaraIT.Training.Domain.Extensions;
using AwaraIT.Training.Domain.Models.Crm;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Models.Crm.SystemEntities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Web.UI;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    public static class IntetestPluginAssignmentOnCreationTest
    {

        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {

                    var clietntD365 = (IOrganizationService)client;

                    var tsPl = new TestPluginInterestContact2(clietntD365);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");

            }
        }
    }

    internal class TestPluginInterestContact2
    {
        private readonly IOrganizationService _service;
        private readonly string _teamName;
        private Logger _log;
        public TestPluginInterestContact2(IOrganizationService service)
        {
            _teamName = "fnt___Колл-центр";
            _service = service;
            Execute();



        }

        private void Execute()
        {

            _log = new Logger(_service);
            try
            {
                var interest = new Interest
                {
                    Status = new OptionSetValue((int)Interest.InterestStepStatus.New),
                    FirstName = "John",
                    LastName = "Doe",
                    MiddleName = "Smith",
                    Phone = "1234567890",
                    Email = "test@example.com",
                    TerritoryReference = new EntityReference("fnt_territory", Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6")),
                    InterestId = Guid.NewGuid()

                };


                // var interest = wrapper.TargetEntity.ToEntity<Interest>();

                if (interest != null && interest.Status != null)
                {
                    if (interest.StatusToEnum == InterestStepStatus.New)
                    {
                        var contact = FindOrCreateContact(_service, interest);

                        interest.ContactReference = contact.ToEntityReference();
                        //_log.INFO($"{nameof(IntetestPluginAssignmentOnCreation)}, контакт назначен. Интерес ID: {interest.Id}, Контакт ID: {contact.Id}");

                        var responsibleUser = GetLeastLoadedUser(_service, interest.TerritoryReference.Id);
                        if (responsibleUser.Id == Guid.Empty)
                        {
                            return;
                        }


                        var ownerId = interest.OwnerId;

                        interest.OwnerId = responsibleUser.ToEntityReference();

                        // var sdf = _service.Create(interest);

                        _log.INFO($"InterestAssignmentPlugin: Владелец интереса назначен - " +
                         $"ID интереса: {interest.Id}, ID владельца: {interest.OwnerId.Id}");
                    }

                }
                else
                {/*
                    _log.ERROR($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)},interest Entity is null in targetEntity");
                    throw new ArgumentNullException($"Error ocurred in {nameof(IntetestPluginAssignmentOnCreation)}, interest Entity is null in targetEntity");*/
                }
            }
            catch (Exception ex)
            {
                /* _log.ERROR($"Ошибка в {nameof(Execute)} {ex.Message}, {ex}");
                 Trace("InterestAssignmentPlugin обнаружила ошибку: {0}", ex.ToString());*/
                throw new InvalidPluginExecutionException($"Ошибка в {nameof(Execute)}: {ex.Message}", ex);
            }

        }

        private Entity FindOrCreateContact(IOrganizationService wrapper, Interest interest)
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

                var contacts = _service.RetrieveMultiple(query).Entities;
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

                    contact.Id = _service.Create(contact);
                    _log.INFO($"Контакт создан и назначен к интересу {contact.Id}");
                    return contact;
                }
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(FindOrCreateContact)} {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(FindOrCreateContact)}: {ex.Message}", ex);
            }
        }

        private Entity GetLeastLoadedUser(IOrganizationService wrapper, Guid territoryId)
        {
            try
            {


                var usersId = GetUserIdListByTeamName(_service, _teamName);

                _log.INFO($"Получены пользователи команды, количество: {usersId.Count}");

                var loadQuery = new QueryExpression("fnt_interest")
                {
                    // ColumnSet = new ColumnSet(true /*Interest.Metadata.OwnerId*/),

                    ColumnSet = new ColumnSet(Interest.Metadata.InterestId, Interest.Metadata.OwnerId),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, usersId.ToArray()),
                                new ConditionExpression(Interest.Metadata.Status, ConditionOperator.Equal, InterestStepStatus.InProgress.ToIntValue()),

                            }
                    }

                };

                // Получаем записи интересов
                var interestRecords = _service.RetrieveMultiple(loadQuery).Entities;

                var ownerId = interestRecords[0].ToEntity<Interest>().OwnerId;

                var statusList = interestRecords.Select(e => e.ToEntity<Interest>());
                // Подсчитываем интересы для каждого пользователя
                var userLoadCounts = interestRecords
                  .GroupBy(record => record.ToEntity<Interest>().OwnerId.Id);


                var conunt = userLoadCounts
                .ToDictionary(g => g.Key, g => g.Count());

                //Получаем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = conunt
                  .OrderBy(entry => entry.Value)
                  .FirstOrDefault().Key;



                return new Entity(User.EntityLogicalName, leastLoadedUserId);
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в {nameof(GetLeastLoadedUser)} {ex.Message}, {ex}");
                throw new Exception($"Ошибка в {nameof(GetLeastLoadedUser)}: {ex.Message}", ex);
            }
        }



        private List<Guid> GetUserIdListByTeamName(IOrganizationService service, string teamName)
        {
            try
            {
                // Query to get users associated with the team using LinkEntity
                var userQuery = new QueryExpression(User.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(User.Metadata.SystemUserId), // Field we want to retrieve
                    LinkEntities =
                    {
                       new LinkEntity(User.EntityLogicalName, TeammembershipNN.EntityLogicalName, "systemuserid", "systemuserid", JoinOperator.Inner)
                       {
                             LinkEntities =
                             {
                                   new LinkEntity(TeammembershipNN.EntityLogicalName, Team.EntityLogicalName, "teamid", "teamid", JoinOperator.Inner)
                                   {
                                       LinkCriteria = new FilterExpression
                                       {
                                              Conditions =
                                              {
                                                new ConditionExpression(Team.Metadata.Name, ConditionOperator.Equal, teamName)
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

