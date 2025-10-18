namespace ChatBotAPI.Models
{
	public class SqsSettings
	{
		public string QueueUrl { get; set; } = string.Empty;
		public string AwsRegion { get; set; } = string.Empty;
		public string? AwsAccessKeyId { get; set; }
		public string? AwsSecretAccessKey { get; set; }
		public int MaxNumberOfMessages { get; set; } = 10;
		public int WaitTimeSeconds { get; set; } = 20;
		public int PollingIntervalSeconds { get; set; } = 5;
	}
}

