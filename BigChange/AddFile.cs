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
using System.Net.Mime;

namespace BigChange
{

    public static class AddFileAttachment
    {
        [FunctionName("AddFileAttachment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

        

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int entityType = data?.entityType;
            int entityId = data?.entityId;
            string fileName = data?.fileName;
            string fileType = data?.fileType;
            byte[] fileBytes = data?.fileBytes;

            var options = new RestClientOptions(Credentials.environmentUrl)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Credentials.wsPath, Method.Get);
            request.AddHeader("Authorization", Credentials.authstring);
            request.AddOrUpdateParameter("Key", Credentials.bcKey);
            request.AddOrUpdateParameter("Action", "addattachments");
            request.AddOrUpdateParameter("EntityType", entityType);
            request.AddOrUpdateParameter("EntityId", entityId);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("attachments", "[{\"Filename\":\""+ fileName +"\",\"Type\": \""+ fileType +"\",\"ConvertToPdf\":false,\"Compressed\": false}]");
            request.AddFile("Files", "/C:/Users/MR96805/OneDrive - Sheffield City Council/test-file-bc.pdf");
            RestResponse response = await client.ExecuteAsync(request);
            JObject responseObject = JObject.Parse(response.Content);

            return new OkObjectResult(responseObject);
        }
    }
}
