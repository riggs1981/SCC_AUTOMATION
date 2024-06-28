using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CrypticWizard.RandomWordGenerator;
using System.Linq;

namespace GenerateUserPassword
{
    public static class PasswordFunction
    {
        [FunctionName("GenerateUserPassword")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            WordGenerator myWordGenerator = new WordGenerator();
            string word1 = myWordGenerator.GetWord(WordGenerator.PartOfSpeech.adj);
            string word2 = myWordGenerator.GetWord(WordGenerator.PartOfSpeech.verb);
            string word3 = myWordGenerator.GetWord(WordGenerator.PartOfSpeech.noun);
            Random ran = new Random();
            int a = ran.Next(10, 99);
            string sc = "*!@&$%";
            int sca = ran.Next(1,6);


            var newPassword = new ReturnClasses();
            newPassword.generatedPassword = char.ToUpper(word1[0]) + word1.Substring(1) + char.ToUpper(word2[0]) + word2.Substring(1) + char.ToUpper(word3[0]) + word3.Substring(1) + a + sc.ElementAt(sca);

            return new OkObjectResult(newPassword);
        }
    }
}
