﻿using ChatBotAPI.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI;
using ChatBotAPI.Helpers;
using System.ClientModel;
using System.Text.Json;

namespace ChatBotAPI.Services
{
	public class MessageService : IMessageService
	{
		private readonly OpenAISettings _openAISettings;
		private ChatContext _chatContext;
		private readonly Helper _helper;
		private readonly IRiskManagerService _riskManagerService;
		

		public MessageService(IOptions<OpenAISettings> openAISettings, ChatContext chatContext, Helper helper, IRiskManagerService riskManagerService)
		{
			_openAISettings = openAISettings.Value;
			_chatContext = chatContext;
			_helper = helper;
			_riskManagerService = riskManagerService;
		}

		public async Task<Response> CompleteMessage(UserMessage message)
		{
			var response = new Response();
			response.SessionId = message.SessionId;
		
			if (string.IsNullOrEmpty(message.SessionId))
			{
				response.SessionId = _helper.GetSessionId();
			}
			if (string.IsNullOrEmpty(message.Message))
			{
				response.ResponseMessage = "Please provide a message.";
				return response;
			}
			var chatResponse = await ProcessMessage(message);
			var messageRequestData = chatResponse.Value.Content[0].Text;
			response.ResponseMessage = messageRequestData;

			var chatRequest = JsonSerializer.Deserialize<ChatRequest>(messageRequestData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			if (chatRequest == null)
			{
				response.ResponseMessage = "Invalid response from OpenAI.";
				return response;
			}

			var passwordRequestValidation = _helper.ValidatePasswordRequestData(chatRequest);
			if (passwordRequestValidation == null)
			{
				response.ResponseMessage = "Unable to process your message please try again later.";
				return response;
			}
            if (!chatRequest.IsPasswordRelated)
            {
				response.ResponseMessage = "Hi! My current capabilities are limited to password resets and account unlocks. Let me know how I can assist!";
				return response;
            }
            if (!passwordRequestValidation.IsValidRequest)
			{
				response.ResponseMessage = passwordRequestValidation.Message ?? "Unable to proces your request, please try again later or provide new request.";
				return response;
			}
            if (passwordRequestValidation.IsUnlockAccountRequest)
            {
				response.ResponseMessage = await _riskManagerService.UnlockPassword(chatRequest);
			} else if (passwordRequestValidation.IsResetPasswordRequest)
			{
				response.ResponseMessage = await _riskManagerService.ResetPassword(chatRequest);
			}
			else
			{
				response.ResponseMessage = "Not a valid request to perform, I can do only either reset password or unlock it.";
			}

			return response;
		}

		private async Task<ClientResult<ChatCompletion>> ProcessMessage(UserMessage message)
		{
			var chatContent  = $"Analyze the user's query to determine if it is related to changing, resetting, updating, or unlocking the password of a user for an application. Validate the extracted values and only return a JSON object with the following keys: 'isPasswordRelated' (boolean), 'action' (validated action such as 'change', 'reset', 'update', or 'unlock'), 'applicationName' (validated application name), and 'userName' (validated user name). User's query: {message.Message}";

			var openAIClient = new OpenAIClient(_openAISettings.ApiKey);
			var chatRequest = ChatMessage.CreateSystemMessage(chatContent);

			// Call OpenAI API to get a response
			var chatResponse = await openAIClient.GetChatClient(_openAISettings.Model).CompleteChatAsync(chatRequest);
			return chatResponse;
		}
	}
}
