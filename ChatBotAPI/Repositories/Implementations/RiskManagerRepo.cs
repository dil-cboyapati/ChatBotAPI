using Dapper;
using Microsoft.Data.SqlClient;

using ChatBotAPI.Helpers;
using ChatBotAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatBotAPI.Repositories
{
	public class RiskManagerRepo : IRiskManagerRepo
	{
		private readonly Helper _helper;
		public RiskManagerRepo(Helper helper)
		{
			_helper = helper;
		}
		public async Task<string> UnlockUserAccount(string userName, string application)
		{
			var dbResponse = new DBResponse();
			var connStr = _helper.GetApplicationDBConnectionString(application);
			var testQuery = DBString.GetUnlockUserQuery(userName);
			//run ms sql db commannd using dapper
			using (var connection = new SqlConnection(connStr))
			{
				connection.Open();
				var query = DBString.GetUnlockUserQuery(userName);
				dbResponse = await connection.QueryFirstAsync<DBResponse>(query);
			}
			return dbResponse.IsSuccess ? dbResponse.SuccessMessage : dbResponse.ErrorMessage;
		}

		public async Task<string> ResetUserAccountPasword(string userName, string application)
		{
			var dbResponse = new DBResponse();
			var connStr = _helper.GetApplicationDBConnectionString(application);
			var testQuery = DBString.GetResetUserPasswordQuery(userName);
			//run ms sql db commannd using dapper
			using (var connection = new SqlConnection(connStr))
			{
				connection.Open();
				var query = DBString.GetResetUserPasswordQuery(userName);
				dbResponse = await connection.QueryFirstAsync<DBResponse>(query);
			}
			return dbResponse.IsSuccess ? dbResponse.SuccessMessage : dbResponse.ErrorMessage;
		}
	}
}
