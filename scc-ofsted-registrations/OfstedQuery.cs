using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;

namespace scc_ofsted_registrations
{
    public static class OfstedQuery
    {
        [FunctionName("ProviderQuery")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",  Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Request for Ofsted Data");

            string RegistrationNo = req.Query["RegistrationNo"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var ProviderQuery = new OfstedQueryData();
            var OfstedResponse = new OfstedResponseData();
            ProviderQuery.OfstedRef = RegistrationNo;

            string connectionDetails = @"Server=" + OfstedConnection.sqlserver +";Database=" +OfstedConnection.sqldb + ";User ID=" + OfstedConnection.sqlUser + "; Password=" + OfstedConnection.sqlPass;
            string selectString = OfstedConnection.query;
            
            SqlConnection sqlSelectConnection = new SqlConnection(connectionDetails);
            JArray valuesList = new JArray();
            sqlSelectConnection.Open();
            SqlCommand getProvider = new SqlCommand(selectString, sqlSelectConnection);
            getProvider.Parameters.AddWithValue("@ProviderRef", RegistrationNo);
            SqlDataReader reader = getProvider.ExecuteReader();
            while (reader.Read())
            {


                int urlId = reader.GetOrdinal("OfstedLink");
                int resultId = reader.GetOrdinal("LastInspectionResult");
                int dateId = reader.GetOrdinal("LastInspectionDate");
                int typeId = reader.GetOrdinal("LastInspectionType");



                OfstedResponse.OfstedURL = reader.GetString(urlId);
                OfstedResponse.InspectionResult = reader.GetString(resultId);
                OfstedResponse.InspectionDate = DateOnly.FromDateTime(reader.GetDateTime(dateId));
                OfstedResponse.InspectionType = reader.GetString(typeId);
                OfstedResponse.ProviderID = RegistrationNo;

            }
     


            return new OkObjectResult(OfstedResponse);
        }
    }
}
