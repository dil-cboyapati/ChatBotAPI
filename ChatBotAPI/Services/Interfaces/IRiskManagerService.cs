using ChatBotAPI.Models;

namespace ChatBotAPI.Services
{
	public interface IRiskManagerService
	{
		Task<string> UnlockPassword(ChatRequest request);
		Task<string> ResetPassword(ChatRequest request);
	}
}
