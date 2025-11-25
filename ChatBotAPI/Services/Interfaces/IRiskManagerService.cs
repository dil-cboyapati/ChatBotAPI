namespace ChatBotAPI.Services
{
	public interface IRiskManagerService
	{
		Task<string> UnlockPassword(string userName, string applicationName, string? environmentName = null);
		Task<string> ResetPassword(string userName, string applicationName, string? environmentName = null);
		Task<string> RunProcessDefinition(string processName, string applicationName, string? environmentName = null);
		Task<string> CloseCampaignForms(int campaignId, string applicationName, string? environmentName = null);
	}
}
