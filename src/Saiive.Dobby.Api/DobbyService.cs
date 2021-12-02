using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Saiive.Dobby.Api.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saiive.Dobby.Api
{
    internal class DobbyService : IDobbyService 
    {
        protected readonly HttpClient _client;
        protected readonly string _userAuthKey;
        protected readonly string _dobbyUrl;

        public DobbyService(IConfiguration config)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(5);

            _userAuthKey = config["DOBBY_USER_AUTH_KEY"];
            _dobbyUrl = config["DOBBY_API_URL"];

            _client.DefaultRequestHeaders.Add("x-user-auth", _userAuthKey);
        }

        private async Task<T> DoPost<T>(string url, object postMessage)
        {
            var response = await _client.PostAsync($"{_dobbyUrl}/{url}", new StringContent(JsonConvert.SerializeObject(postMessage), System.Text.Encoding.UTF8, "application/json"));

            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(data)!;
        }
        public async Task<ApiResponse> AddVaultForUser(string vaultId)
        {
            var postMessage = new AddVaultToUserRequest
            {
                VaultId = vaultId
            };

            var response = await DoPost<ApiResponse>("user/vault", postMessage);
            return response!;
        }

        public async Task<CreateNotificationTriggerResponse> CreateNotificationTrigger(string vaultId, int ratio, string type, string info)
        {
            //TODO
            var postMessage = new CreateNotificationTriggerRequest
            {
                VaultId = vaultId
            };

            var response = await DoPost<CreateNotificationTriggerResponse>("user/vault", postMessage);
            return response!;
        }

        public async Task<ApiResponse> DeleteVaultForUser(string vaultId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetNotificationRequestResponse> GetNotificationTriggers()
        {
            throw new NotImplementedException();
        }
    }
}
