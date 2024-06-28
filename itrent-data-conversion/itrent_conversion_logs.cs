using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace itrent_data_conversion
{
    public static class itrent_check_conversion_logs
    {
        [FunctionName("check_conversion_logs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string itrentQueueId = req.Query["itrentQueueId"];
            var conversionLogs = new itrent.CHK_CONV_NEWRequest();
            conversionLogs.P_USER_NM = itrentCredentials.u;
            conversionLogs.P_USER_PWD = itrentCredentials.p;
            conversionLogs.P_QUEUE_ID = itrentQueueId;



            var itrentendpointconfig = new System.ServiceModel.EndpointAddress(itrentCredentials.e);
            var client = new itrent.ETADM086SSPortTypeClient();
            client.Endpoint.Address = itrentendpointconfig;

            var response = client.CHK_CONV_NEW(conversionLogs);

            var logOutputs = new itrent.CHK_CONV_NEWResponse();
            logOutputs = response;
            var responseObject = JsonConvert.SerializeObject(logOutputs);
            var responseJson = JObject.Parse(responseObject);
            

            req.HttpContext.Response.Headers.Add("Content-type", "Application/JSON");
            return new OkObjectResult(responseJson);
        }
    }
}
