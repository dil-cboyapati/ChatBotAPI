using Amazon.SQS;
using Amazon.SQS.Model;
using ChatBotAPI.Models;
using Microsoft.Extensions.Options;

namespace ChatBotAPI.Services.BackgroundServices
{
	public class SqsPollingService : BackgroundService
	{
		private readonly ILogger<SqsPollingService> _logger;
		private readonly SqsSettings _sqsSettings;
		private readonly IAmazonSQS _sqsClient;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public SqsPollingService(
			ILogger<SqsPollingService> logger,
			IOptions<SqsSettings> sqsSettings,
			IAmazonSQS sqsClient,
			IServiceScopeFactory serviceScopeFactory)
		{
			_logger = logger;
			_sqsSettings = sqsSettings.Value;
			_sqsClient = sqsClient;
			_serviceScopeFactory = serviceScopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("SQS Polling Service is starting.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await PollMessagesAsync(stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred while polling SQS messages.");
				}

				// Wait before polling again
				await Task.Delay(TimeSpan.FromSeconds(_sqsSettings.PollingIntervalSeconds), stoppingToken);
			}

			_logger.LogInformation("SQS Polling Service is stopping.");
		}

		private async Task PollMessagesAsync(CancellationToken cancellationToken)
		{
			var request = new ReceiveMessageRequest
			{
				QueueUrl = _sqsSettings.QueueUrl,
				MaxNumberOfMessages = _sqsSettings.MaxNumberOfMessages,
				WaitTimeSeconds = _sqsSettings.WaitTimeSeconds,
				MessageAttributeNames = new List<string> { "All" },
				AttributeNames = new List<string> { "All" }
			};

			var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);

			if (response.Messages.Any())
			{
				_logger.LogInformation($"Received {response.Messages.Count} message(s) from SQS.");

				foreach (var message in response.Messages)
				{
					await ProcessMessageAsync(message, cancellationToken);
				}
			}
		}

		private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation($"Processing message: {message.MessageId}");
				_logger.LogInformation($"Message Body: {message.Body}");

				// Create a new scope for scoped services
				using (var scope = _serviceScopeFactory.CreateScope())
				{
					// TODO: Add your message processing logic here
					// Example: Get your services and process the message
					// var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
					// await messageService.ProcessMessageAsync(message.Body);

					// If processing is successful, delete the message from the queue
					await DeleteMessageAsync(message.ReceiptHandle, cancellationToken);
				}

				_logger.LogInformation($"Successfully processed message: {message.MessageId}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing message: {message.MessageId}");
				// Message will remain in queue and be retried based on queue's visibility timeout
			}
		}

		private async Task DeleteMessageAsync(string receiptHandle, CancellationToken cancellationToken)
		{
			try
			{
				var deleteRequest = new DeleteMessageRequest
				{
					QueueUrl = _sqsSettings.QueueUrl,
					ReceiptHandle = receiptHandle
				};

				await _sqsClient.DeleteMessageAsync(deleteRequest, cancellationToken);
				_logger.LogInformation("Message deleted from queue.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting message from queue.");
				throw;
			}
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("SQS Polling Service is stopping gracefully.");
			await base.StopAsync(cancellationToken);
		}
	}
}

