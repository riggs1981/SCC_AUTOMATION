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
using System.Xml;
using Newtonsoft.Json.Linq;

namespace SCCCrb
{
    public static class CRB_Individual
    {
        [FunctionName("CheckCRB")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string DOB = data?.DOB;
            string Surname = data?.Surname;
            string certNo = data?.certNo;

            var options = new RestClientOptions("https://secure.crbonline.gov.uk/crsc/api/status/" + certNo);
  

            var client = new RestClient(options);
            
            var request = new RestRequest("", Method.Get);
            Console.WriteLine(client.BuildUri(request));
            request.AddParameter("dateOfBirth", DOB);
            request.AddParameter("surname",Surname);
            request.AddParameter("hasAgreedTermsAndConditions", "true");
            request.AddParameter("organisationName", "SheffieldCityCouncil");
            request.AddParameter("employeeSurname", "IT");
            request.AddParameter("employeeForename", "SCC");

            Console.WriteLine(client.ToString());
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response.Content);
            var responseString = JsonConvert.SerializeXmlNode(xmlDocument);
            var responseJSON = JsonConvert.DeserializeObject(responseString);


            return new OkObjectResult(responseJSON);
        }
    }
}
