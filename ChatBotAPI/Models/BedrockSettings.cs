namespace ChatBotAPI.Models
{
	public class BedrockSettings
	{
		public string ModelId { get; set; } = string.Empty;
		public string AwsRegion { get; set; } = string.Empty;
		public string? AwsAccessKeyId { get; set; }
		public string? AwsSecretAccessKey { get; set; }
		public int MaxTokens { get; set; } = 2000;
		public double Temperature { get; set; } = 0.7;
		public double TopP { get; set; } = 0.9;
	}
}

