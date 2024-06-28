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

    public static class AddPersonToContact
    {
        [FunctionName("AddPersonToContact")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int contactId = data?.contactId;
            string title = data?.title;
            string forename = data?.forename;
            string surname = data?.surname;
            string phone = data?.phone;
            string email = data?.email;
            string position = data?.position;
            string department = data?.department;
            string PrimaryContact = data?.primaryContact;


            var options = new RestClientOptions(Credentials.environmentUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Credentials.wsPath, Method.Get);
            request.AddHeader("Authorization", Credentials.authstring);
            request.AddOrUpdateParameter("Key", Credentials.bcKey);
            request.AddOrUpdateParameter("Action", "ContactSavePerson");
            request.AddOrUpdateParameter("ContactId", contactId);
            request.AddOrUpdateParameter("Email", email);
            request.AddOrUpdateParameter("MainUser", "false");
            request.AddOrUpdateParameter("FirstName", forename);
            request.AddOrUpdateParameter("LastName", surname);
            request.AddOrUpdateParameter("Title",title);
            request.AddOrUpdateParameter("Position", position);
            request.AddOrUpdateParameter("Department", department);
            request.AddOrUpdateParameter("Phone",phone);
            request.AddOrUpdateParameter("MainUser",PrimaryContact);



            RestResponse response = await client.ExecuteAsync(request);
            JObject responseObject = JObject.Parse(response.Content);

            return new OkObjectResult(responseObject);
        }
    }
}
