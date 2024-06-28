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
    public static class itrent_data_conversion
    {
        [FunctionName("Execute_new_conversion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string itrentOperation =  data?.itrentOperation;
            string itrentPayload = data?.itrentPayload;
            string itrentOrg = data?.itrentOrg;
            string itrentDataIdentifier = data?.itrentDataIdentifier;
            string fileSeperator = data?.itrentFileSeperator;

            if (fileSeperator == "") { fileSeperator = ","; }
            else { };


            var dataLoad = new itrent.RUN_CONV_NEWRequest();
            dataLoad.P_CONV_FILE = itrentPayload;
            dataLoad.P_CONV_TYPE = itrentOperation;
            dataLoad.P_FS = fileSeperator;
            dataLoad.P_PEOPLE_ID = itrentDataIdentifier;
            dataLoad.P_ORG_NAME = itrentOrg;
            dataLoad.P_CONV_DIR = "";
            dataLoad.P_USER_PWD = itrentCredentials.p;
            dataLoad.P_USER_NM = itrentCredentials.u;



            var itrentendpointconfig = new System.ServiceModel.EndpointAddress(itrentCredentials.e);
            var client = new itrent.ETADM086SSPortTypeClient();
            client.Endpoint.Address = itrentendpointconfig;
            var response = client.RUN_CONV_NEW(dataLoad);

            var dataloadOutcome = new itrent.RUN_CONV_NEWResponse();
            dataloadOutcome = response;
            var responseObject = JsonConvert.SerializeObject(dataloadOutcome);
            var responseJson = JObject.Parse(responseObject);
            req.HttpContext.Response.Headers.Add("Content-type", "Application/JSON");



            return new OkObjectResult(responseJson);
        }
    }
}
