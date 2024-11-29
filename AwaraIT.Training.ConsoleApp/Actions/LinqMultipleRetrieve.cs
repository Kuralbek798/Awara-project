/*
using AwaraIT.Training.ConsoleApp;
using AwaraIT.Training.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Actions
{
    internal class LinqMultipleRetrieve
    {
        internal static void Run()
        {
            try
            {
                using(var client = Program.GetCrmClient())
                {
                    var clientD365 = (IOrganizationService)client;

                    var query = new QueryExpression("contact")
                    {
                        ColumnSet = new ColumnSet(Contact.Metadata.MobilePhone, Contact.Metadata.FirstName, Contact.Metadata.LastName)
                    };
                    query.Criteria.AddCondition(Contact.Metadata.FirstName, ConditionOperator.NotNull);
                    var res = clientD365.RetrieveMultiple(query);
                    var contacts = res.Entities.Select(e => e.ToEntity<Contact>()).ToList();
                    contacts.OrderBy(e => e.FirstName);
                    contacts.ForEach(contact => Console.WriteLine($"Contact: {contact.FirstName} mobile phone {contact.MobilePhone}"));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
*/