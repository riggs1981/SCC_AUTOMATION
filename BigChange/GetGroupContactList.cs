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

    public static class GetGroupContactList
    {
        [FunctionName("GetGroupContactList")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int groupId = data?.groupId;


            var options = new RestClientOptions(Credentials.environmentUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Credentials.wsPath, Method.Get);
            request.AddHeader("Authorization", Credentials.authstring);
            request.AddOrUpdateParameter("Key", Credentials.bcKey);
            request.AddOrUpdateParameter("Action", "ContactListByGroupPaged");
            request.AddOrUpdateParameter("groupid", groupId);
            request.AddOrUpdateParameter("V2", 1);
            request.AddOrUpdateParameter("page", 0);
            request.AddOrUpdateParameter("pagesize", 5000);
            RestResponse response = await client.ExecuteAsync(request);
            JObject responseObject = JObject.Parse(response.Content);

            return new OkObjectResult(responseObject);
        }
    }
}
