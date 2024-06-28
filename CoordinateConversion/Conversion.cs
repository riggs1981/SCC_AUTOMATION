using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GeoUK;
using GeoUK.Coordinates;
using GeoUK.Ellipsoids;
using GeoUK.Projections;
using Convert = GeoUK.Convert;

namespace CoordinateConversion
{
    public static class BNGConverter
    {
        [FunctionName("LatLonToBng")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

       

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            double lon = data?.lon;
            double lat = data?.lat;
            
            var latLong = new LatitudeLongitude(lat, lon);

            var cartesian = Convert.ToCartesian(new Wgs84(), latLong);
            var bngCartesian = Transform.Etrs89ToOsgb36(cartesian);
            var bngEN = Convert.ToEastingNorthing(new Airy1830(), new BritishNationalGrid(), bngCartesian);



            return new OkObjectResult(bngEN);
        }
    }

    public static class LatLongConverter
    {
        [FunctionName("BNGToLatLong")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            double x = data?.x;
            double y = data?.y;

            Cartesian cartesian = Convert.ToCartesian(new Airy1830(),
            new BritishNationalGrid(),
            new EastingNorthing(x, y));

            Cartesian wgsCartesian = Transform.Osgb36ToEtrs89(cartesian);
            LatitudeLongitude wgsLatLong = Convert.ToLatitudeLongitude(new Wgs84(), wgsCartesian);



            return new OkObjectResult(wgsLatLong);
        }
    }
}
