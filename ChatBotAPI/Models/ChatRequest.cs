namespace ChatBotAPI.Models
{
	public class ChatRequest
	{
		public bool IsPasswordRelated { get; set; }
		public string ApplicationName { get; set; }
		public string UserName { get; set; }
		public string Action { get; set; }
	}
}
