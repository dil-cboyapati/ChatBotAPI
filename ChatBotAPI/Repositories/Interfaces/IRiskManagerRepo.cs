using ChatBotAPI.Models;

namespace ChatBotAPI.Repositories
{
	public interface IRiskManagerRepo
	{
		Task<string> UnlockUserAccount(string userName, string application, string? environmentName = null);
		Task<string> ResetUserAccountPasword(string userName, string application, string? environmentName = null);
		Task<string> RunProcessDefinition(string processName, string application, string? environmentName = null);
		Task<string> CloseCampaignForms(int campaignId, string application, string? environmentName = null);
	}
}
