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

    public static class ListPeopleForContact
    {
        [FunctionName("ListPeopleForContact")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int contactId = data?.contactId;


            var options = new RestClientOptions(Credentials.environmentUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Credentials.wsPath, Method.Get);
            request.AddHeader("Authorization", Credentials.authstring);
            request.AddOrUpdateParameter("Key", Credentials.bcKey);
            request.AddOrUpdateParameter("Action", "contactlistperson");
            request.AddOrUpdateParameter("contactId", contactId);
            request.AddOrUpdateParameter("FilterMainUser", "false");
            RestResponse response = await client.ExecuteAsync(request);
            
            log.LogInformation(response.Content);
            
            JObject responseObject = JObject.Parse(response.Content);

            return new OkObjectResult(responseObject);
        }
    }
}
