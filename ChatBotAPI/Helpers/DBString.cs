using ChatBotAPI.Models;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatBotAPI.Helpers
{
	public static class DBString
	{

		private const string UnlockUserSQL =
			@"BEGIN TRY 
				DECLARE @UserId INT = NULL;
				SELECT @UserId = ID FROM USERS WHERE USER_NAME_TX = '@UserName'
				IF @UserId IS NULL
				BEGIN
				 SELECT 0 AS IsSuccess
				 , NULL AS SuccessMessage
				 , 'User does not exists. Please provide existing user name to unlock.' AS ErrorMessage
				END
				ELSE
				BEGIN
					BEGIN TRANSACTION;
				
					UPDATE USERS
					SET IS_LOCKED_OUT_IN='N' where ID = @UserId
						
					SELECT 1 AS IsSuccess
					 , 'Successfully unlocked the user.' AS SuccessMessage
					 , NULL AS ErrorMessage
				
					 COMMIT TRANSACTION;
				
				END
				
				END TRY
				BEGIN CATCH
					ROLLBACK transaction
					 SELECT 0 AS IsSuccess
				 , NULL AS SuccessMessage
				 , 'Something did not work as expected. Give it another try in a bit' AS ErrorMessage
				END CATCH";
		private const string ResetUserPasswordSQL =
			@"BEGIN TRY 
				DECLARE @UserId INT = NULL;
				SELECT @UserId = ID FROM USERS WHERE USER_NAME_TX = '@UserName'
				IF @UserId IS NULL
				BEGIN
				 SELECT 0 AS IsSuccess
				 , NULL AS SuccessMessage
				 , 'User does not exists. Please provide existing user name to reset the password.' AS ErrorMessage
				END
				ELSE
				BEGIN
					BEGIN TRANSACTION;
				
					UPDATE USERS
					SET PASSWORD_TX='3g3lzg+EMuqe37goPeHLrnK47H71vvG82T9VbCYRVNXxCkClZe/PFjEW/UG2AscLmJsKxK25ecXRrXJLrY0lEf/xNFr6eD3nASSQNKe26jOvil4JxQgooct87Yj474NlZRoC7inMr+9OGpDqu3TVaRjsBMfor87bVzta3odvHyk='
					,SALT_TX='Uj8ssL9jOr33thIQbnZQrTv20Vj29IcDpetdTmG+MeO7m9yV9mvDElCSh8Wp5B5NhTRd+qz0dHf21OPYeOfToJz9JioUKY8aJ5RnXve6Ko67yF8P06WrRUowAnadrYT9MCnmNGqHw+OuNdWsupT03vDA8Y3qcukUfCyDiUNDgxY='
					,IS_LOCKED_OUT_IN='N'
					,CHANGE_PASSWORD_IN='Y'
					WHERE ID = @UserId
						
					SELECT 1 AS IsSuccess
					 , 'Reset password completed for the user.' AS SuccessMessage
					 , NULL AS ErrorMessage
				
					 COMMIT TRANSACTION;
				
				END
				
				END TRY
				BEGIN CATCH
					ROLLBACK transaction
					 SELECT 0 AS IsSuccess
				 , NULL AS SuccessMessage
				 , 'Something did not work as expected. Give it another try in a bit' AS ErrorMessage
				END CATCH";
		public static string GetUnlockUserQuery(string userName)
		{
			return UnlockUserSQL.Replace("@UserName", userName);
		}

		public static string GetResetUserPasswordQuery(string userName)
		{
			return ResetUserPasswordSQL.Replace("@UserName", userName);
		}

		private const string RunProcessDefinitionSQL =
			@"BEGIN TRY 
				DECLARE @ProcessId INT = NULL;
				SELECT @ProcessId = ID FROM PROCESS_DEFINITIONS WHERE PROCESS_NAME_TX = '@ProcessName'
				IF @ProcessId IS NULL
				BEGIN
				 SELECT 0 AS IsSuccess
				 , NULL AS SuccessMessage
				 , 'Process definition does not exist. Please provide a valid process name.' AS ErrorMessage
				END
				ELSE
				BEGIN
					BEGIN TRANSACTION;
				
					-- Dummy script to run process definition
					-- In a real scenario, this would execute the actual process definition logic
					UPDATE PROCESS_DEFINITIONS
					SET LAST_RUN_DT = GETDATE()
					, STATUS_TX = 'RUNNING'
					WHERE ID = @ProcessId
					
					-- Simulate process execution delay
					WAITFOR DELAY '00:00:01'
					
					UPDATE PROCESS_DEFINITIONS
					SET STATUS_TX = 'COMPLETED'
					WHERE ID = @ProcessId
						
					SELECT 1 AS IsSuccess
					 , 'Process definition executed successfully.' AS SuccessMessage
					 , NULL AS ErrorMessage
				
					 COMMIT TRANSACTION;
				
				END
				
				END TRY
				BEGIN CATCH
					ROLLBACK transaction
					 SELECT 0 AS IsSuccess
				 , NULL AS SuccessMessage
				 , 'Something did not work as expected. Give it another try in a bit' AS ErrorMessage
				END CATCH";

		public static string GetRunProcessDefinitionQuery(string processName)
		{
			return RunProcessDefinitionSQL.Replace("@ProcessName", processName);
		}

		private const string CloseCampaignFormsSQL =
			@"BEGIN TRY 
				DECLARE @CampaignId INT = @RelateId;
				DECLARE @RowsAffected INT = 0;
				
				-- Check if campaign exists (optional validation)
				-- You can add campaign validation here if needed
				
				BEGIN TRANSACTION;
			
				UPDATE RESPONSE_CONTAINER 
				SET STATUS_CD = 'RCSTATUS_CLOSED', UPDATE_USER_TX = 'RM-Agent-S'
				WHERE RELATE_ID = @CampaignId 
				AND STATUS_CD IN ('RCSTATUS_START')

				SET @RowsAffected = @@ROWCOUNT;


				UPDATE RESPONSE_CONTAINER 
				SET STATUS_CD = 'RCSTATUS_CLOSED', UPDATE_USER_TX = 'RM-Agent-A'
				WHERE RELATE_ID = @CampaignId 
				AND STATUS_CD IN ('RCSTATUS_AVAILABLE')
				
				SET @RowsAffected = @RowsAffected + @@ROWCOUNT;
				
				IF @RowsAffected > 0
				BEGIN
					SELECT 1 AS IsSuccess
					 , CONCAT('Successfully closed ', @RowsAffected, ' form(s) for the campaign.') AS SuccessMessage
					 , NULL AS ErrorMessage
				END
				ELSE
				BEGIN
					SELECT 1 AS IsSuccess
					 , 'No forms found with START or AVAILABLE status for the campaign.' AS SuccessMessage
					 , NULL AS ErrorMessage
				END
			
				 COMMIT TRANSACTION;
			
			END TRY
			BEGIN CATCH
				ROLLBACK transaction
				 SELECT 0 AS IsSuccess
			 , NULL AS SuccessMessage
			 , 'Something did not work as expected. Give it another try in a bit' AS ErrorMessage
			END CATCH";

		public static string GetCloseCampaignFormsQuery(int campaignId)
		{
			return CloseCampaignFormsSQL.Replace("@RelateId", campaignId.ToString());
		}

		//CREATE A SQL Script to check users based on username and if the user exists only once then and update to unlock the user





	}
}
