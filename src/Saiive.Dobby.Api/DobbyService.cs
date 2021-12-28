using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Saiive.Dobby.Api.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Saiive.Dobby.Api
{
    internal class DobbyService : IDobbyService
    {
        protected readonly string _dobbyUrl;

        public IConfiguration Config { get; }

        public DobbyService(IConfiguration config)
        {
            _dobbyUrl = config["DOBBY_API_URL"];
            Config = config;
        }

        private HttpClient SetupClient(string authKey)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            if (!String.IsNullOrEmpty(authKey))
            {
                client.DefaultRequestHeaders.Add("x-user-auth", authKey);
            }
            return client;
        }

        private async Task<T> DoPost<T>(string url, string authKey, object postMessage)
        {
            using var client = SetupClient(authKey);
            var response = await client.PostAsync($"{_dobbyUrl}/{url}", new StringContent(JsonConvert.SerializeObject(postMessage), System.Text.Encoding.UTF8, "application/json"));

            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(data)!;
        }
        private async Task<T> DoDelete<T>(string url, string authKey, object postMessage)
        {
            using var client = SetupClient(authKey);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_dobbyUrl}/{url}"),
                Content = new StringContent(JsonConvert.SerializeObject(postMessage), System.Text.Encoding.UTF8, "application/json")
            };
         
            var response = await client.SendAsync(request);
            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(data)!;
        }
        private async Task<T> DoGet<T>(string url, string authKey)
        {
            using var client = SetupClient(authKey);
            var response = await client.GetAsync($"{_dobbyUrl}/{url}");

            var data = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(data)!;
        }
        public async Task<GetUserResponse> SetupUser(string locale)
        {
            var postMessage = new CreateUserRequest
            {
                Language = locale,
                Theme = "light"
            };

            var response = await DoPost<GetUserResponse>("setup", null,  postMessage);

            await CreateNotificationGateway(response.UserId!, "webhook", Config["WEBHOOK_URL"]);

            return response!;
        }

        public async Task<ApiResponse> AddVaultForUser(string authKey, string vaultId)
        {
            var postMessage = new AddVaultToUserRequest
            {
                VaultId = vaultId
            };

            var response = await DoPost<ApiResponse>("user/vault", authKey,  postMessage);
            return response!;
        }

        public async Task<CreateNotificationTriggerResponse> CreateNotificationTrigger(string authKey, string vaultId, int ratio, string type)
        {
            var gateways = await GetNotificationGateways(authKey);

            var postMessage = new CreateNotificationTriggerRequest
            {
                VaultId = vaultId,
                Gateways = gateways.Data.Select(a => a.GatewayId).ToList(),
                Ratio = ratio,
                Type = type
            };

            var response = await DoPost<CreateNotificationTriggerResponse>("user/notification", authKey, postMessage);
            return response!;
        }

        public async Task<ApiResponse> DeleteVaultForUser(string authKey, string vaultId)
        {
            var postMessage = new AddVaultToUserRequest
            {
                VaultId = vaultId
            };

            var response = await DoDelete<ApiResponse>("user/vault", authKey, postMessage);
            return response!;
        }

        public async Task<GetNotificationRequestResponse> GetNotificationTriggers(string authKey)
        {
            return await DoGet<GetNotificationRequestResponse>("user/notification", authKey);
        }

        public async Task<GetNotificationGatewaysResponse> GetNotificationGateways(string authKey)
        {
            return await DoGet<GetNotificationGatewaysResponse>("user/gateways", authKey);
        }

        public async Task<ApiResponse> CreateNotificationGateway(string authKey, string type, string data)
        {
            var postMessage = new CreateNotificationGatewayRequest
            {
                Value = data,
                Type = type
            };

            var response = await DoPost<ApiResponse>("user/gateways", authKey, postMessage);

            return response!;
        }
    }
}
