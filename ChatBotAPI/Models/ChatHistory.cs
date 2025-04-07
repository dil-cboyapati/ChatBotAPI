namespace ChatBotAPI.Models
{
	public class ChatHistory
	{
		public List<string> Messages { get; set; } = new();
		public ChatRequest ChatRequest { get; set; } = new();
	}
}
