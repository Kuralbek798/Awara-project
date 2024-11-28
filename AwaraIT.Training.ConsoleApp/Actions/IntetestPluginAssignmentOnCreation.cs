
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

namespace AwaraIT.Training.ConsoleApp.Actions
{
	public static class IntetestPluginAssignmentOnCreation
	{
      
        internal static void Run()
		{
			try
			{
				using(var client = Program.GetCrmClient())
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
                Guid teamId = GetTeamId(_service, _teamName);
                _log.INFO($"Получен ID команды: {teamId}");

                var usersId = GetUserIdListInTeam(_service, teamId);
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
                               // new ConditionExpression(Interest.Metadata.Territory, ConditionOperator.Equal, teritoryId)
                            }
                    }

                    /* Criteria = new FilterExpression
                     {
                         FilterOperator = LogicalOperator.And,
                         Conditions =
                         {
                             new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, usersId.ToArray()),
                             new ConditionExpression(EntityCommon.StatusCode, ConditionOperator.Equal, 797_720_002 *//*InterestStepStatus.InProgress.ToIntValue()*//*),
                             //new ConditionExpression(Interest.Metadata.TerritoryReference, ConditionOperator.Equal, territoryId)
                         }
                     },
                     Distinct = true*/
                };

                // Получаем записи интересов
                var interestRecords = _service.RetrieveMultiple(loadQuery).Entities;

                var ownerId = interestRecords[0].ToEntity<Interest>().OwnerId;

                var statusList = interestRecords.Select(e => e.ToEntity<Interest>());
                // Подсчитываем интересы для каждого пользователя
                var userLoadCounts = interestRecords
                  .GroupBy(record => record.ToEntity<Interest>().OwnerId.Id)
                  .ToDictionary(g => g.Key, g => g.Count());

                //Получаем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = userLoadCounts
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

                var guids = service.RetrieveMultiple(membershipQuery).Entities
                           .Select(m => m.ToEntity<Teammembership>().SystemUserId)
                           .ToList();
               // _log.INFO($"list usersId {DataForLogs.GetGuidsString(guids)}");

                return guids;
            }
            catch (Exception ex)
            {
                _log.ERROR($"Ошибка в методе {nameof(GetUserIdListInTeam)}: {ex.Message},  {ex}");
                throw new Exception($"Ошибка в {nameof(GetUserIdListInTeam)}: {ex.Message}", ex);
            }
        }





/*
        public void Execute()
        {
             _logger = new Logger(_service);

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
                    TerritoryReference = new EntityReference("fnt_territory", Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"))
                };
                var email = interest.Email;
                var phone = interest.Phone;
                var teritoryId = interest.TerritoryReference.Id;
                // поиск или создание контакта
                var contact = FindOrCreateContact(_service, email, phone, interest);

                // Присвоение контакта
                interest.ContactReference = contact.ToEntityReference();
                _logger.INFO($"InterestAssignmentPlugin: Contact assigned - " +
                            $"Interest [LogicalName: , ID:]; " +
                            $"Contact [LogicalName: {contact.LogicalName}, ID: {contact.Id}]");

                // Получение пользователя с наименьшей нагрузкой
                var responsibleUser = GetLeastLoadedUser(_service, teritoryId);
                if (responsibleUser.Id == Guid.Empty)
                {
                    return;
                }
                interest.OwnerId = responsibleUser.ToEntityReference();
                _service.Create(interest);
                _logger.INFO($"InterestAssignmentPlugin: Interest owner assigned - " +
                      $"Interest ID: {interest.Id}, Owner ID: {interest.OwnerId.Id}");           
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                _logger.ERROR("CreateInterestPlugin", ex.ToString(), "interest", Guid.Parse("4fd197ce-80a6-ef11-8a6a-000d3a5c09a6"));
            }
        }

        private Entity FindOrCreateContact(IOrganizationService service, string email, string phone, Interest interest)
        {
            try
            {
                // Поиск контакта по email и телефону
                var query = new QueryExpression(Contact.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(true)
                };
                //поиск идет по email и телефону поэтому RetrieveMultiple так как Retrieve работает только по одному ID
                query.Criteria.AddCondition(Contact.Metadata.Email, ConditionOperator.Equal, email);
                query.Criteria.AddCondition(Contact.Metadata.Phone, ConditionOperator.Equal, phone);

                var contacts = service.RetrieveMultiple(query).Entities;
                if (contacts.Any())
                {
                    var contact = contacts.First();
                    _logger.INFO($"Contact assigned to interest {contact.Id}");
                    return contact;
                }
                else
                {
                    //Создание нового контакта
                    var contact = new Contact
                    {
                        Email = email,
                        Id = Guid.NewGuid(),
                        FirstName = interest.FirstName,
                        MiddleName = interest.MiddleName,
                        Phone = phone,
                        TerritoryReference = interest.TerritoryReference,
                        LastName = interest.LastName,

                    };
                    contact.Id = service.Create(contact);
                    _logger.INFO($"Contact created and assigned to interest {contact.Id}");
                    return contact;
                }
            }
            catch (Exception ex)
            {

                _logger.ERROR($"method {nameof(FindOrCreateContact)} {ex.ToString()}, contactEntityLogicalName: {Contact.EntityLogicalName}, interestId: {interest.Id}");

                throw new Exception($"method {nameof(FindOrCreateContact)}" + ex.Message, ex);
            }
        }

        private Entity GetLeastLoadedUser(IOrganizationService service, Guid territoryId)
        {
            try
            {
                Guid teamId = GetTeamId(service, _teamName);              

                var usersId = GetUserIdListInTeam(service, teamId);              

                var loadQuery = new QueryExpression(Interest.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(EntityCommon.OwnerId),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                           new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, usersId.ToArray()),
                            new ConditionExpression(EntityCommon.StatusCode, ConditionOperator.Equal, InterestStepStatus.InProgress.ToIntValue()),
                           // new ConditionExpression(Interest.Metadata.TerritoryReference, ConditionOperator.Equal, territoryId)
                        }
                    },
                    //Distinct = true
                };

                // Получаем записи интересов
                var interestRecords = service.RetrieveMultiple(loadQuery).Entities;

                // Подсчитываем интересы для каждого пользователя
                var userLoadCounts = interestRecords
                  .GroupBy(record => record.GetAttributeValue<EntityReference>(EntityCommon.OwnerId).Id)
                  .ToDictionary(g => g.Key, g => g.Count());

                //Получаем пользователя с наименьшей нагрузкой
                var leastLoadedUserId = userLoadCounts
                  .OrderBy(entry => entry.Value)
                  .FirstOrDefault().Key;
                                

                return new Entity(User.EntityLogicalName, leastLoadedUserId);
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Ошибка в {nameof(GetLeastLoadedUser)}: {ex.Message}", ex);
            }
        }

        private Guid GetTeamId(IOrganizationService service, string teamName)
        {
            try
            {
                // Создаем запрос для получения команды по имени
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
                    _logger.ERROR($"method {nameof(GetTeamId)} team is null, teameName: {teamName}");
                    throw new ArgumentNullException($"method {nameof(GetTeamId)} team is null, teameName: {teamName}");
                }

                return team.TeamId;
            }
            catch (Exception ex)
            {
                _logger.ERROR($"method {nameof(GetTeamId)} {ex.ToString()}, WorkGroup.EntityLogicalName: {WorkGroup.EntityLogicalName},  teameName: {teamName}");

                throw new Exception($"method {nameof(GetTeamId)}" + ex.Message, ex);
            }
        }

        private List<Guid> GetUserIdListInTeam(IOrganizationService service, Guid teamId)
        {
            try
            {
                // Создаем запрос для получения всех пользователей, входящих в команду
                var membershipQuery = new QueryExpression(Teammembership.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Teammembership.Metadata.SystemUserId),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                    {
                        new ConditionExpression(Teammembership.Metadata.TeamId, ConditionOperator.Equal, teamId) // Условия для фильтрации по ID команды
                    }
                    }
                };

                // Выполняем запрос к членству команды
                var memberships = service.RetrieveMultiple(membershipQuery).Entities
                    .Select(m => m.ToEntity<Teammembership>().SystemUserId)
                    .ToList();
                memberships.ForEach(e => Console.WriteLine(e));
                // Извлекаем и возвращаем список идентификаторов пользователей
                return memberships;
            }
            catch (Exception ex)
            {
                _logger.ERROR($"method {nameof(GetUserIdListInTeam)} {ex.ToString()}, Teammembership.EntityLogicalName: {Teammembership.EntityLogicalName},  teamId: {teamId}");

                throw new Exception($"method {nameof(GetUserIdListInTeam)}" + ex.Message, ex);
            }
        }*/
    }
}

