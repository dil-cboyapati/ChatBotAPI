using ChatBotAPI.Models;

namespace ChatBotAPI.Services
{
	public interface IMessageService
	{
		Task<Response> CompleteMessage(UserMessage message);
	}
}
