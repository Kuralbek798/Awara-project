
using AwaraIT.Kuralbek.Plugins.Helpers;
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

                        var usersIdList = GetUserIdListByTeamName(_service);
                        _log.INFO($"Получены пользователи команды, количество: {usersIdList.Count}");

                        // Условия для поиска записей
                        var conditionsExpressions = PluginHelper.SetConditionsExpressions(usersIdList, Interest.Metadata.Status, InterestStepStatus.InProgress.ToIntValue());
                        // Получаем наименее загруженного пользователя для сущности Interest
                        var responsibleUser = PluginHelper.GetLeastLoadedEntity(_service, conditionsExpressions, Interest.EntityLogicalName, EntityCommon.OwnerId, _log);

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

        /// <summary>
        /// Находит или создает контакт на основе предоставленной информации об интересе.
        /// </summary>
        /// <param name="wrapper">Контекст выполнения плагина.</param>
        /// <param name="interest">Информация об интересе.</param>
        /// <returns>Сущность контакта.</returns>
        /// <exception cref="Exception">Выбрасывается при возникновении ошибки во время поиска или создания контакта.</exception>
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

                var contacts = wrapper.RetrieveMultiple(query).Entities;
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

                    contact.Id = wrapper.Create(contact);

                    return contact;
                }
            }
            catch (Exception ex)
            {
                throw;
                /*_log.ERROR($"Error in method {nameof(FindOrCreateContact)} of {nameof(IntetestPluginAssignmentOnCreation)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(FindOrCreateContact)} method of {nameof(IntetestPluginAssignmentOnCreation)}.", ex);*/
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

                // Выполняем запрос и получаем сущности пользователей извлекаем идентификаторы пользователей из полученных сущностей
                var userIds = service.RetrieveMultiple(userQuery).Entities.Select(e => e.ToEntity<User>().SystemUserId).ToList();


                return userIds;
            }
            catch (Exception ex)
            {
                throw;
                /*_log.ERROR($"Error in method {nameof(GetUserIdListByTeamName)} of {nameof(IntetestPluginAssignmentOnCreation)}: {ex.Message}, {ex}");
                throw new InvalidPluginExecutionException($"An error occurred in the {nameof(GetUserIdListByTeamName)} method of {nameof(IntetestPluginAssignmentOnCreation)}.", ex);*/
            }
        }

    }
}

