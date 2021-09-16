using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PiRecordingControls
{
    public class ApiUtil
    {
        private IConfiguration _configuration;
        private readonly string _endPoint;
        private readonly string _headerKey;

        public ApiUtil(IConfiguration configuration)
        {
            _configuration = configuration;
            _endPoint = configuration["DataAPI:EndPoint"];
            _headerKey = configuration["DataAPI:HeaderKey"];
        }

        /// <summary>
        /// Lähettää kasvodatan projektin rajapintaan myöhempää tilastollista analyysiä varten.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SendDataToAPI(ApiData data)
        {
            Console.WriteLine($"Sending data to API:\n{data}");
            if (bool.Parse(_configuration["Debugging:SendDataToAPI"]) == true)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(data.ToString(), UTF8Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    content.Headers.Add("Ocp-Apim-Subscription-Key", _headerKey);
                    var response = await client.PostAsync(_endPoint, content);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            else
            {
                return "Debugging:SendDataToAPI is set to false";
            }
        }
    }
}
