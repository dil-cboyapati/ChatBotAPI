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
				else if (passwordRequestValidation.IsValidRequest)
				{
					passwordRequestValidation.IsResetPasswordRequest = CheckValueExistsInList(passwordRequest.Action, applicationData.ResetPasswordKeywords);
					passwordRequestValidation.IsUnlockAccountRequest = CheckValueExistsInList(passwordRequest.Action, applicationData.UnlockAccountKeywords);
				}
			}
			else
			{
				passwordRequestValidation.IsValidRequest = false;
				passwordRequestValidation.Message = $"Not able to find application name. Please provide request with application name";
			}

			return passwordRequestValidation;
		}

		public string? GetApplicationDBConnectionString(string applicationName, string? environmentName = null)
		{
			//check applicatoinName in clientDataConfig list  by ignoring case
			if (string.IsNullOrEmpty(applicationName))
			{
				return null;
			}


			// Get connection string based on environment
			var connectionString = GetConnectionStringForEnvironment(applicationName, environmentName);
			if (!string.IsNullOrEmpty(connectionString))
			{
				// Replace INITIAL CATALOG with application name (case-insensitive)
				connectionString = System.Text.RegularExpressions.Regex.Replace(
					connectionString,
					@"INITIAL\s+CATALOG\s*=\s*[^;]+",
					$"INITIAL CATALOG={applicationName}",
					System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			}

			return connectionString;
		}

		private string? GetConnectionStringForEnvironment(string? applicationName, string? environmentName)
		{
			// If no application name or environment specified, return null
			if (string.IsNullOrEmpty(applicationName) || string.IsNullOrEmpty(environmentName))
			{
				return null;
			}

			// Step 1: Validate application name exists in ClientDataConfig
			return _clientDataConfig.FirstOrDefault(x => CheckValueExistsInList(environmentName, x.Environments) && CheckValueExistsInList(applicationName, x.ApplicationName))?.ConnectionString;
		}

		public ClientDataConfig? GetAppicationDataFromClientDataConfig(string applicationName)
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

