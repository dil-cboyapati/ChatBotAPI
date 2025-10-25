using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using ChatBotAPI.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace ChatBotAPI.Services
{
	public class BedrockService : IBedrockService
	{
		private readonly BedrockSettings _bedrockSettings;
		private readonly IAmazonBedrockRuntime _bedrockClient;
		private readonly ILogger<BedrockService> _logger;

		public BedrockService(
			IOptions<BedrockSettings> bedrockSettings,
			IAmazonBedrockRuntime bedrockClient,
			ILogger<BedrockService> logger)
		{
			_bedrockSettings = bedrockSettings.Value;
			_bedrockClient = bedrockClient;
			_logger = logger;
		}

		public async Task<string> ProcessMessage(UserMessage message)
		{
			try
			{
				var chatContent = $"Analyze the user's query to determine if it is related to changing, resetting, updating, or unlocking the password of a user for an application. Validate the extracted values and only return a JSON object with the following keys: 'isPasswordRelated' (boolean), 'action' (validated action such as 'change', 'reset', 'update', or 'unlock'), 'applicationName' (validated application name), 'userName' (validated user name), and 'environmentName' (validated environment name such as 'DEV', 'QA', 'PROD', 'PREPROD'). User's query: {message.Message}";

				// Prepare the request payload for Claude/Bedrock
				var requestPayload = PrepareBedrockRequest(chatContent);

				// Convert to JSON and then to bytes
				var requestJson = JsonSerializer.Serialize(requestPayload);
				var requestBytes = Encoding.UTF8.GetBytes(requestJson);

				// Create the invoke request
				var invokeRequest = new InvokeModelRequest
				{
					ModelId = _bedrockSettings.ModelId,
					Body = new MemoryStream(requestBytes),
					ContentType = "application/json",
					Accept = "application/json"
				};

			_logger.LogInformation($"Invoking Bedrock model: {_bedrockSettings.ModelId}");

			// Call Bedrock API
			var response = await _bedrockClient.InvokeModelAsync(invokeRequest);
			_logger.LogInformation($"Response");

			// Parse the response
				var responseBody = await ParseBedrockResponse(response);

				_logger.LogInformation("Successfully received response from Bedrock");

				return responseBody;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error calling AWS Bedrock");
				throw;
			}
		}

		private object PrepareBedrockRequest(string prompt)
		{
			// This payload format works for Claude models (anthropic.claude-v2, anthropic.claude-3-sonnet, etc.)
			// Adjust based on your specific model
			return new
			{
				anthropic_version = "bedrock-2023-05-31",
				max_tokens = _bedrockSettings.MaxTokens,
				temperature = _bedrockSettings.Temperature,
				top_p = _bedrockSettings.TopP,
				messages = new[]
				{
					new
					{
						role = "user",
						content = prompt
					}
				}
			};
		}

	private async Task<string> ParseBedrockResponse(InvokeModelResponse response)
	{
		using var reader = new StreamReader(response.Body);
		var responseJson = await reader.ReadToEndAsync();

		_logger.LogInformation($"Bedrock raw response: {responseJson}");

		// Parse based on Claude response format with case-insensitive property matching
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		};
		
		var responseObject = JsonSerializer.Deserialize<BedrockClaudeResponse>(responseJson, options);

		if (responseObject?.Content != null && responseObject.Content.Length > 0)
		{
			_logger.LogInformation($"Extracted content: {responseObject.Content[0].Text}");
			return responseObject.Content[0].Text;
		}

		_logger.LogError($"Failed to parse response. ResponseObject: {responseObject != null}, Content: {responseObject?.Content?.Length ?? 0}");
		throw new Exception($"Invalid response from Bedrock. Response was: {responseJson}");
	}

		// Response model for Claude
		private class BedrockClaudeResponse
		{
			public string? Id { get; set; }
			public string? Type { get; set; }
			public string? Role { get; set; }
			public ContentBlock[]? Content { get; set; }
			public string? Model { get; set; }
			public string? StopReason { get; set; }
			public Usage? Usage { get; set; }
		}

		private class ContentBlock
		{
			public string? Type { get; set; }
			public string Text { get; set; } = string.Empty;
		}

		private class Usage
		{
			public int InputTokens { get; set; }
			public int OutputTokens { get; set; }
		}
	}
}

