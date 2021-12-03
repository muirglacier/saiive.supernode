using Saiive.Dobby.Api.Model;
using System.Threading.Tasks;

namespace Saiive.Dobby.Api
{
    public interface IDobbyService
    {
        Task<GetUserResponse> SetupUser(string locale);

        Task<ApiResponse> AddVaultForUser(string authKey, string vaultId);
        Task<ApiResponse> CreateNotificationGateway(string authKey, string type, string data);
        Task<ApiResponse> DeleteVaultForUser(string authKey, string vaultId);

        Task<CreateNotificationTriggerResponse> CreateNotificationTrigger(string authKey, string vaultId, int ratio, string type);
        Task<GetNotificationRequestResponse> GetNotificationTriggers(string authKey);

        Task<GetNotificationGatewaysResponse> GetNotificationGateways(string authKey);
    }
}
