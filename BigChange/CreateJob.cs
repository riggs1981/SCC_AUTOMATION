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

    public static class CreateJob
    {
        [FunctionName("CreateJob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

          

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
       
            string JobType = data?.JobType;
            string jobGroup = data?.jobGroup;
            string jobCategory = data?.jobCategory;
            int contactId = data?.contactId;
            string jobDescription = data?.jobDescription;
            string jobStart = data?.jobStart;
            int resourceId = data?.resourceId;
            dynamic customFields = data?.customFields;



            var options = new RestClientOptions(Credentials.environmentUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Credentials.wsPath, Method.Get);
            request.AddHeader("Authorization", Credentials.authstring);
            request.AddOrUpdateParameter("Key", Credentials.bcKey);
            request.AddOrUpdateParameter("Action", "Jobsave");
            request.AddOrUpdateParameter("JobType", JobType);
            request.AddOrUpdateParameter("jobGroup", jobGroup);
            request.AddOrUpdateParameter("jobCategory ", jobCategory);
            request.AddOrUpdateParameter("contactId", contactId);
            request.AddOrUpdateParameter("jobDescription", jobDescription);
            request.AddOrUpdateParameter("jobStart", jobStart);
            request.AddOrUpdateParameter("resourceId", resourceId);
          

            foreach (var field in customFields) {
                string fieldName = field["key"];
                string fieldValue = field["value"];
            request.AddOrUpdateParameter("cust_" + fieldName,fieldValue);
            }

            RestResponse response = await client.ExecuteAsync(request);
            JObject responseObject = JObject.Parse(response.Content);

            return new OkObjectResult(responseObject);
        }
    }
}
