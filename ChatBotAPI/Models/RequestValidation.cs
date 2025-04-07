namespace ChatBotAPI.Models
{
	public class RequestValidation
	{
		public bool IsValidRequest { get; set; }
        public bool IsUnlockAccountRequest { get; set; }
        public bool IsResetPasswordRequest { get; set; }
        public List<string> InvalidProps { get; set; }
        public string Message { get; set; }
    }
}
