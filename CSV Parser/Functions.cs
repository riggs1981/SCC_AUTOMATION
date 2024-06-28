using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChoETL;
using System.Text;
using BarcodeLib;
using BarcodeStandard;
using System.Buffers.Text;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CustomFunctions
{
    public static class CSVParser
    {
        [FunctionName("CSVParser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("CSV Parser Executed");

            string csvData = req.Query["CSVData"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            csvData = csvData ?? data?.CSVData;
            byte[] dataToDecode = Convert.FromBase64String(csvData);
            string decodedString = Encoding.UTF8.GetString(dataToDecode);
    
            StringBuilder sb = new StringBuilder();
            using (var p = ChoCSVReader.LoadText(decodedString).WithFirstLineHeader().MayHaveQuotedFields()
                )
            {
                using (var w = new ChoJSONWriter(sb))
                    w.Write(p);
            }

            
            var jsonResponse = JsonConvert.DeserializeObject(sb.ToString());
            return new OkObjectResult(jsonResponse);
        }
    }

    public static class BarcodeGenerator
    {
        [FunctionName("BarcodeGenerator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Barcode Generator was executed");

            string barcodeData = req.Query["barcodeData"];
            string barcodeType = req.Query["barcodeType"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            barcodeData = barcodeData ?? data?.barcodeData;
            barcodeType = barcodeType ?? data?.barcodeType;


            Barcode barcode = new Barcode();
            BarcodeLib.TYPE barcodeTypeEnum = new BarcodeLib.TYPE();
            barcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;
          
            switch(barcodeType)
            {
                case "CODE39": 
                    {
                        barcodeTypeEnum = BarcodeLib.TYPE.CODE39;
                        break;
            
                    }
                case "CODE11":
                    {
                        barcodeTypeEnum = BarcodeLib.TYPE.CODE11;
                        break;

                    }
                case "CODE128":
                    {
                        barcodeTypeEnum = BarcodeLib.TYPE.CODE128;
                        break;

                    }

            };


            barcode.Encode(barcodeTypeEnum, barcodeData);
            Console.WriteLine(barcode);
            

            

            return new OkObjectResult(barcode);
        }
    }

    public static class RegexReplace
    {
        [FunctionName("RegexReplace")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Regex replace");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string input =  data?.input;
            string regexPattern = data?.regexPattern;
            string replacement = data?.replacement;

            var regexResult = Regex.Replace(input, regexPattern, replacement);
            // Manipulate the response data as applicable before returning it
            JObject output = new JObject
            {
                ["output"] = regexResult
            };



            return new OkObjectResult(output);
        }
    }
}
