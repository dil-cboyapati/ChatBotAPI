using ChatBotAPI.Models;

namespace ChatBotAPI.Services
{
	public interface IBedrockService
	{
		Task<string> ProcessMessage(UserMessage message);
	}
}

