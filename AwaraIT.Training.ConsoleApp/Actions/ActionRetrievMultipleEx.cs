/*
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwaraIT.Training.ConsoleApp;
using AwaraIT.Training.Domain.Models.Crm.Entities;

using System.IdentityModel.Protocols.WSTrust;
using System.Web.Services.Description;

namespace AwaraIT.Training.ConsoleApp.Actions
{
    internal class ActionRetrievMultipleEx
    {
        internal static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {
                    #region RetrieveMultiple делаем выборку всех записей из таблицы  "ColumnSet = new ColumnSet(true)"

                    //------------------------------RetrieveMultiple делаем выборку всех записей из таблицы "ColumnSet = new ColumnSet(true)",
                    // а потом перед отправкой запроса добавляем фильтр: query.Criteria.AddCondition(new ConditionExpression(Contact.Metadata.FullName, ConditionOperator.Equal, "Simson Alekseev"));
                    // ColumnSet устанавливается на new ColumnSet(true), что означает,
                    // что возвращаются все поля(все колонки) для каждого найденного контакта
                    var clietntD365 = (IOrganizationService)client;

                    var query = new QueryExpression(Interest.EntityLogicalName)
                    {
                        ColumnSet = new ColumnSet(true)
                    };
                    var res = clietntD365.RetrieveMultiple(query);
                    var results = res.Entities.Select(e => e.ToEntity<Interest>()).ToList();

                    if (results.Count == 0)
                    {
                        Console.WriteLine("No records found.");
                    }
                    else
                    {
                        foreach (var interest in results)
                        {
                            //var statusValue = interest.GetAttributeValue<OptionSetValue>(Interest.Metadata.Status);
                            //var territory = interest.GetAttributeValue<EntityReference>(Interest.Metadata.Territory);

                            //string statusText = statusValue != null ? ((StatusEnum)statusValue.Value).ToString() : "Unknown";
                            //string territoryName = territory != null ? territory.Name : "Unknown";

                            Console.WriteLine($"Interest: {interest.FirstName}, {interest.LastName}, interest.Status: {interest.Status}, interesStatusEnum {interest.StatusEnum} interest.Territory: {interest.Territory.LogicalName}");
                        }
                    }


                    //Привязываем объект клиента(клиента CRM) к переменной clietntD365,
                    //используя интерфейс IOrganizationService для работы с данными Dataverse.
                    #endregion


                    *//*     var clietntD365 = (IOrganizationService)client;

                         //--------------------------------RetrieveMultiple делаем выборку данных по нескольким условиям

                         // Создаем новый объект FilterExpression с логическим оператором "Или" (Or).
                         // Это будет использоваться для фильтрации результатов выборки.
                         var filter = new FilterExpression(LogicalOperator.Or);

                         // Добавляем условие, чтобы выборка включала записи с полным именем контакта, равным "Simson Alekseev".
                         filter.AddCondition(Contact.Metadata.FullName, ConditionOperator.Equal, "Simson Alekseev");

                         // Добавляем еще одно условие, чтобы выборка включала записи, созданные за последние 1 день.
                         // Используется оператор ConditionOperator.LastXDays для фильтрации по дате создания.
                         filter.AddCondition(new ConditionExpression(EntityCommon.CreatedOn, ConditionOperator.LastXDays, 1));

                         // Создаем новый QueryExpression для сущности Contact (Контакт),
                         // устанавливая оконный набор (ColumnSet) в true, что означает, что будут извлечены все доступные поля для каждой записи.
                         var query = new QueryExpression(Contact.EntityLogicalName)
                         {
                             ColumnSet = new ColumnSet(true),
                             Criteria = filter // Применяем вышеуказанный фильтр к критериям запроса.
                         };

                         // Выполняем запрос к Dataverse с использованием метода RetrieveMultiple, который реализует выборку нескольких записей.
                         // В переменной res будут храниться все записи, которые соответствуют заданному запросу.
                         var res = clietntD365.RetrieveMultiple(query);

                         // Преобразуем извлеченные записи из результата запроса в список объектов типа Contact.
                         // Используется Select для проекции каждой сущности в экземпляр класса Contact.
                         var contacts = res.Entities.Select(e => e.ToEntity<Contact>()).ToList();

                         // Перебираем все извлеченные контакты и выводим их полные имена в консоль.
                         foreach (var contact in contacts)
                         {
                             Console.WriteLine($"Contact: {contact.FullName}");
                         }*//*
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception message : {e.Message}. Exception: {e}");

            }
        }
    }
}
*/