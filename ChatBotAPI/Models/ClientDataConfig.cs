namespace ChatBotAPI.Models
{
	public class ClientDataConfig
	{
		public List<string> ApplicationName { get; set; }
		public string DatabaseName { get; set; }
		public string ConnectionString { get; set; }
		public List<string> UnlockAccountKeywords { get; set; }
		public List<string> ResetPasswordKeywords { get; set; }
		public List<string> ForceChangePasswordKeywords { get; set; }
	}
}
