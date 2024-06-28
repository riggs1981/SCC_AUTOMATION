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
using System.ServiceModel;
using itrent;

namespace itrent_data_conversion
{
    public static class itrent_check_conversion_status
    {
        [FunctionName("check_conversion_status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string itrentQueueId = req.Query["itrentQueueId"];
            var conversionStatusCheck = new itrent.CHK_CONV_STATUSRequest();
            conversionStatusCheck.P_USER_NM = itrentCredentials.u;
            conversionStatusCheck.P_USER_PWD = itrentCredentials.p;
            conversionStatusCheck.P_QUEUE_ID = itrentQueueId;

            var itrentendpointconfig = new System.ServiceModel.EndpointAddress(itrentCredentials.e);
            var client = new itrent.ETADM086SSPortTypeClient();
            client.Endpoint.Address = itrentendpointconfig;

            var response = client.CHK_CONV_STATUS(conversionStatusCheck);

            var dataloadOutcome = new itrent.CHK_CONV_STATUSResponse();
            dataloadOutcome = response;
            var responseObject = JsonConvert.SerializeObject(dataloadOutcome);
            var responseJson = JObject.Parse(responseObject);
            

            req.HttpContext.Response.Headers.Add("Content-type", "Application/JSON");
            return new OkObjectResult(responseJson);
        }
    }
}
