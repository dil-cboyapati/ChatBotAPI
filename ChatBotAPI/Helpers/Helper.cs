using ChatBotAPI.Models;
using Microsoft.Extensions.Options;
using System;

namespace ChatBotAPI.Helpers
{
	public class Helper
	{
		private readonly List<ClientDataConfig> _clientDataConfig;

		public Helper(IOptions<List<ClientDataConfig>> clientDataCofig)
		{
			_clientDataConfig = clientDataCofig.Value;
		}
		public string GetSessionId()
		{
			return Guid.NewGuid().ToString();
		}

		public RequestValidation ValidatePasswordRequestData(ChatRequest passwordRequest)
		{
			var passwordRequestValidation = new RequestValidation();

			var applicationData = GetAppicationDataFromClientDataConfig(passwordRequest.ApplicationName);

			if (applicationData is not null)
			{
				if (string.IsNullOrEmpty(passwordRequest.UserName))
				{
					passwordRequestValidation.IsValidRequest = false;
				}
				else
				{
					passwordRequestValidation.IsValidRequest = true;
				}

				if (string.IsNullOrEmpty(passwordRequest.Action))
				{
					passwordRequestValidation.IsValidRequest = false;
				}
				else if(passwordRequestValidation.IsValidRequest)
				{
					passwordRequestValidation.IsResetPasswordRequest = CheckValueExistsInList(passwordRequest.Action, applicationData.ResetPasswordKeywords);
					passwordRequestValidation.IsUnlockAccountRequest = CheckValueExistsInList(passwordRequest.Action, applicationData.UnlockAccountKeywords);
				}
			}
			else
			{
				passwordRequestValidation.IsValidRequest = false;
				passwordRequestValidation.Message = $"Provided applcation(${passwordRequest.ApplicationName}) is not in the list. Please provide request with valid application";
			}

			return passwordRequestValidation;
		}

		public string GetApplicationDBConnectionString(string applicationName)
		{
			//check applicatoinName in clientDataConfig list  by ignoring case
			if (string.IsNullOrEmpty(applicationName))
			{
				return null;
			}

			var clientData = _clientDataConfig.FirstOrDefault(x => x.ApplicationName.Contains(applicationName.ToUpper()));
			if (clientData == null)
			{
				return null;
			}
			return clientData.ConnectionString;
		}

		public ClientDataConfig GetAppicationDataFromClientDataConfig(string applicationName)
		{
			if (string.IsNullOrEmpty(applicationName))
			{
				return null;
			}

			return _clientDataConfig.FirstOrDefault(x => CheckValueExistsInList(applicationName, x.ApplicationName));
		}

		public bool CheckValueExistsInList(string value, List<string> list)
		{
			return list.FindIndex(x => x.Equals(value, StringComparison.OrdinalIgnoreCase)) >= 0;
		}
	}
}
