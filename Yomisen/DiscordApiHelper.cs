using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Yomisen
{
    public static class DiscordApiHelper
    {
        /// <summary>
        /// EmailとPasswordでトークンを取得します
        /// </summary>
        public static async Task<string> LogInAsync(string mail, string password)
        {
            var baseUrl = "https://discordapp.com/api/";
            var appName = "Yomisen";
            var discordAccount = new { email = mail, password = password };
            var json = JsonConvert.SerializeObject(discordAccount);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await GetClient(appName).PostAsync(new Uri($@"{baseUrl}/auth/login"), content);

            if (res.IsSuccessStatusCode == true)
            {
                var resJson = await res.Content.ReadAsStringAsync();
                var deserializedJson = JsonConvert.DeserializeAnonymousType(resJson, new { token = "" });
                return deserializedJson.token;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ヘッダーの追加
        /// </summary>
        public static HttpClient GetClient(string appName)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", appName);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
