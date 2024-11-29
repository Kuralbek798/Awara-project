using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using AwaraIT.Kuralbek.Plugins;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using AwaraIT.Training.Domain.Extensions;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    internal class ActionEntityReferencedEx
    {
        static Dictionary<string, string> _contactAliasDiction = new Dictionary<string, string>();
        static Dictionary<string, Type> _typesDictionary = new Dictionary<string, Type>();
        public static void SetDictionaryValues()
        {
            _contactAliasDiction.Add("firstname", "contact.firstname");
            _contactAliasDiction.Add("contactid", "contact.contactid");
            _typesDictionary.Add("firstname", typeof(string));
            _typesDictionary.Add("contactid", typeof(Guid));
        }
        internal static void Run()
        {
            try
            {
                SetDictionaryValues();
                using (var client = Program.GetCrmClient())
                {

                    var clietntD365 = (IOrganizationService)client;


                    // Создаем новый QueryExpression для сущности "Contact".
                    // Используется для запроса записей из Dataverse, где "Contact.EntityLogicalName" представляет собой логическое имя сущности контактов.
                    var query = new QueryExpression(Contact.EntityLogicalName)
                    {
                        // Устанавливаем ColumnSet в false, что означает, что основные атрибуты контакта не будут возвращены.
                        // Это эффективно, когда нужны только связанные данные.
                        ColumnSet = new ColumnSet(false)
                    };

                    // Добавляем условие к запросу для фильтрации записей по идентификатору.
                    // Используем ConditionOperator.Equal для точного совпадения, указывая определенный GUID контакта.
                    // Это условие выбирает конкретную запись контакта, имеющую данный контактный идентификатор.
                    query.Criteria.AddCondition("contactid", ConditionOperator.Equal, new Guid("a38d671e-b4a1-ef11-8a6a-00224805a2d2"));


                    // Создаем связь (LinkEntity) с другой записью в той же таблице контактов.
                    // Эта связь используется для получения данных из родительского контакта.
                    // "parentcustomerid" — это поле в текущем контакте, указывающее на связанный записанный идентификатор контакта.
                    // "contactid" — это целевое поле в той же таблице, с которым устанавливается связь.
                    var linc = query.AddLink(Contact.EntityLogicalName, "parentcustomerid", "accountid");

                    // Определяем, какие атрибуты нужно извлечь из связанного (родительского) контакта.
                    // "contactid" и "firstname" — это столбцы, которые будут возвращены в результате запроса.
                    linc.Columns = new ColumnSet("contactid", "firstname");

                    // Устанавливаем алиас "contact" для связанной записи.
                    // Это используется, чтобы различать столбцы одинакового имени из разных мест и избежать возможных конфликтов имен.
                    linc.EntityAlias = "contact";

                    // Выполняем запрос к Dataverse с использованием подготовленного QueryExpression.
                    // "clietntD365.RetrieveMultiple(query)" возвращает множество записей, которые соответствуют критериям запроса.
                    var res = clietntD365.RetrieveMultiple(query);

                    // Извлекаем массив сущностей из результата запроса.
                    // Этот массив содержит контакты, которые соответствуют критериям и с которыми была установлена связь.
                    var contacts = res.Entities;

                    // Получаем значение имени (firstname) из первой записи в извлеченном массиве сущностей.
                    // "GetAliasedValue<string>" используется для получения значения столбца, обозначенного алиасом, что позволяет точно указать, из какой таблицы получено значение.
                    var firstName = contacts[0].GetAliasedValue<string>("contact.firstname");
                    var contactId = contacts[0].GetAliasedValue<Guid>("contact.contactid");


                    Console.WriteLine("firstName: " + firstName.ToString());
                    Console.WriteLine("contactId: " + contactId.ToString());

                    var contactList = contacts.ToList();

                    foreach (KeyValuePair<string, string> item in _contactAliasDiction)
                    {

                        var type = _typesDictionary[item.Key];
                        var alias = item.Value;

                        var result = contactList.Select(contact => contact.GetAliasedValue(alias, type))
                            .FirstOrDefault();

                        Console.WriteLine($"{item.Key}:{result.ToString()}");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");

            }
        }
    }
}
