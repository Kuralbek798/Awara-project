using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Web.Services.Description;

namespace AwaraIT.Training.ConsoleApp.Actions
{
    public class MetadataCheck
    {
        private static IOrganizationService service; // Add this line to define the 'service' variable

        public static void Run()
        {
            try
            {
                using (var client = Program.GetCrmClient())
                {

                    var service = (IOrganizationService)client;

                    if (service == null)
                    {
                        throw new InvalidOperationException("The service object is not initialized.");
                    }

                    var request = new RetrieveEntityRequest
                    {
                        LogicalName = "fnt_contact",
                        EntityFilters = EntityFilters.Attributes
                    };

                    var response = (RetrieveEntityResponse)service.Execute(request);
                    foreach (var attribute in response.EntityMetadata.Attributes)
                    {
                        Console.WriteLine($"Attribute Name: {attribute.LogicalName}, Type: {attribute.AttributeType}");
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
