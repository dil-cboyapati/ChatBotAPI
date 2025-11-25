using ChatBotAPI.Repositories;

namespace ChatBotAPI.Services
{
	public class RiskManagerService : IRiskManagerService
	{
		private readonly IRiskManagerRepo _riskManagerRepo;
		

		public RiskManagerService(IRiskManagerRepo riskManagerRepo)
		{
			_riskManagerRepo = riskManagerRepo;
		}

	public async Task<string> UnlockPassword(string userName, string applicationName, string? environmentName = null)
	{
		var unlockResponse = await _riskManagerRepo.UnlockUserAccount(userName, applicationName, environmentName);
		return unlockResponse;
	}

	public async Task<string> ResetPassword(string userName, string applicationName, string? environmentName = null)
	{
		var resetResponse = await _riskManagerRepo.ResetUserAccountPasword(userName, applicationName, environmentName);
		return resetResponse;
	}

	public async Task<string> RunProcessDefinition(string processName, string applicationName, string? environmentName = null)
	{
		var processResponse = await _riskManagerRepo.RunProcessDefinition(processName, applicationName, environmentName);
		return processResponse;
	}

	public async Task<string> CloseCampaignForms(int campaignId, string applicationName, string? environmentName = null)
	{
		var closeResponse = await _riskManagerRepo.CloseCampaignForms(campaignId, applicationName, environmentName);
		return closeResponse;
	}
	}
}
