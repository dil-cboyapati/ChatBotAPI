using ChatBotAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI;
using ChatBotAPI.Helpers;
using System.ClientModel;
using System.Text.Json.Serialization;
using System.Text.Json;
using ChatBotAPI.Repositories;

namespace ChatBotAPI.Services
{
	public class RiskManagerService : IRiskManagerService
	{
		private readonly IRiskManagerRepo _riskManagerRepo;
		

		public RiskManagerService(IRiskManagerRepo riskManagerRepo)
		{
			_riskManagerRepo = riskManagerRepo;
		}

		public async Task<string> UnlockPassword(ChatRequest request)
		{
			var unlockResponse = await _riskManagerRepo.UnlockUserAccount(request.UserName, request.ApplicationName);
			return unlockResponse;
		}

		public async Task<string> ResetPassword(ChatRequest request)
		{
			var resetResponse = await _riskManagerRepo.ResetUserAccountPasword(request.UserName, request.ApplicationName);
			return resetResponse;
		}
	}
}
