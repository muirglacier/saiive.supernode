using Saiive.Dobby.Api.Model;
using System.Threading.Tasks;

namespace Saiive.Dobby.Api
{
    public interface IDobbyService
    {
        Task<ApiResponse> AddVaultForUser(string vaultId);
        Task<ApiResponse> DeleteVaultForUser(string vaultId);

        Task<CreateNotificationTriggerResponse> CreateNotificationTrigger(string vaultId, int ratio, string type, string info);
        Task<GetNotificationRequestResponse> GetNotificationTriggers();
    }
}
