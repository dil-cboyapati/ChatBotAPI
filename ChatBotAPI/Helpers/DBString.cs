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
				SELECT @UserId
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
				SELECT @UserId
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

		//CREATE A SQL Script to check users based on username and if the user exists only once then and update to unlock the user





	}
}
