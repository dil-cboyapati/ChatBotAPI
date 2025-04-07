using System.Collections.Concurrent;

namespace ChatBotAPI.Models
{
	public class ChatContext
	{
        public ConcurrentDictionary<string, ChatHistory> ChatContextDetails { get; set; }
    }
}
