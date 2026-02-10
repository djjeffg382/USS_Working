using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace MOO.Services
{

    public static class TeamsNotificationSvc
    {
        /// <summary>
        /// URL to send POST.
        /// </summary>
        public const string TEAMSERRORURL = "https://prod-83.westus.logic.azure.com/workflows/a5ee29bcea444722a7b4f2af8954c601/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=tEf9Dil0sRnBXlSnu6xWQEE5DObRF891FFC1htCwdX4";

        /// <summary>
        /// Sends a message to the Teams notification
        /// </summary>
        /// <param name="url">URL of the teams</param>
        /// <param name="teamsNote">Teams Note value</param>
        /// <returns></returns>
        public static async Task SendMessage(string url, Models.TeamsNotification teamsNote)
        {
            StringContent content = new(JsonConvert.SerializeObject(teamsNote), Encoding.UTF8, "application/json");

            System.Net.Http.HttpClient httpClient = new();

            using var response = await httpClient.PostAsync(url, content);
            string apiResponse = await response.Content.ReadAsStringAsync();
        }
    }
        
}



