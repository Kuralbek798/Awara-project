
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
using System.Web.UI;

namespace AwaraIT.Training.ConsoleApp.Actions
{
	public static class TestAction
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
        private Logger _logger;
        public TestPluginInterestContact2(IOrganizationService service)
        {
            _teamName = "fnt___Колл-центр";
            _service = service;
            Execute();
        }

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
                var responsibleUser = GetLeastLoadedUser(_service, _teamName, teritoryId);

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

        private Entity GetLeastLoadedUser(IOrganizationService service, string teamName, Guid teritoryId)
        {
            try
            {
                // Получаем идентификатор команды по имени
                var teamId = GetTeamId(service, teamName);
                _logger.INFO($"Teamid obtained: {teamId}");

                // Получаем всех пользователей, принадлежащих этой команде
                var users = GetUserIdListInTeam(service, teamId);
                _logger.INFO($"Users of Team obtained, users count: {users.Count}");

                // Группируем пользователей по их Id и считаем количество связанных записей
                Dictionary<Guid, int> userLoadCounts = new Dictionary<Guid, int>();

                // Выполняем запрос, чтобы получить количество открытых записей интереса или сделок для этого пользователя
                var loadQuery = new QueryExpression(Interest.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Interest.Metadata.InterestId, Interest.Metadata.OwnerId),
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(EntityCommon.OwnerId, ConditionOperator.In, users.ToArray()),
                                new ConditionExpression(EntityCommon.StatusCode, ConditionOperator.Equal, (int) Interest.InterestStepStatus.InProgress),
                                new ConditionExpression(Interest.Metadata.TerritoryReference, ConditionOperator.Equal, teritoryId)
                            }
                    }
                };
                var interestRecords = service.RetrieveMultiple(loadQuery).Entities;

                foreach (var interestRecord in interestRecords)
                {
                    var userId = interestRecord.ToEntity<Interest>().OwnerId.Id;
                    if (userId != null)
                    {

                        if (userLoadCounts.ContainsKey(userId))
                        {
                            userLoadCounts[userId]++;
                        }
                        else
                        {
                            userLoadCounts[userId] = 1;
                        }
                    }
                }

                // Шаг 4: Находим пользователя с минимальной загрузкой
                var leastLoadedUserId = userLoadCounts.OrderBy(entry => entry.Value).FirstOrDefault().Key;

                // Находим и возвращаем информацию о пользователе
                return new Entity(User.EntityLogicalName, leastLoadedUserId);
            }
            catch (Exception ex)
            {
                _logger.ERROR($"method {nameof(GetLeastLoadedUser)} {ex.ToString()}");

                throw new Exception($"method {nameof(GetLeastLoadedUser)}" + ex.Message, ex);

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
        }
    }
}

