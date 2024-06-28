using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace BigChange
{

    public static class AddRelatedContact
    {
        [FunctionName("AddRelatedContact")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

          

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
       
            string title = data?.title;
            string forename = data?.forename;
            string surname = data?.surname;
            string addressLine1 = data?.addressLine1;
            string town = data?.town;
            string postcode = data?.postcode;
            double longitude = data?.longitude;
            double latitude = data?.latitude;
            string phone = data?.phone;
            string contactGroup = data?.contactGroup;
            string email = data?.email; 
            string contactNote = data?.contactNote;
            string relatedContactId = data?.relatedContactId;
            dynamic customFields = data?.customFields;



            var options = new RestClientOptions(Credentials.environmentUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Credentials.wsPath, Method.Get);
            request.AddHeader("Authorization", Credentials.authstring);
            request.AddOrUpdateParameter("Key", Credentials.bcKey);
            request.AddOrUpdateParameter("Action", "ContactSave");
            request.AddOrUpdateParameter("contactName",forename + " " + surname);
            request.AddOrUpdateParameter("contactFirstName",forename);
            request.AddOrUpdateParameter("contactLastName",surname);
            request.AddOrUpdateParameter("contactStreet",addressLine1);
            request.AddOrUpdateParameter("contactTown",town);
            request.AddOrUpdateParameter("contactPostcode",postcode);
            request.AddOrUpdateParameter("contactLat",latitude);
            request.AddOrUpdateParameter("contactLng",longitude);
            request.AddOrUpdateParameter("contactPhone",phone);
            request.AddOrUpdateParameter("contactEmail",email);
            request.AddOrUpdateParameter("contactExtra",contactNote);
            request.AddOrUpdateParameter("contactGroup",contactGroup);
            request.AddOrUpdateParameter("contactParentId", relatedContactId);


            foreach (var field in customFields) {
                string fieldName = field["key"];
                string fieldValue = field["value"];
            request.AddOrUpdateParameter("cust_" + fieldName,fieldValue);
            }

            RestResponse response = await client.ExecuteAsync(request);
            log.LogInformation(response.Content);
            JObject responseObject = JObject.Parse(response.Content);

            return new OkObjectResult(responseObject);
        }
    }
}
