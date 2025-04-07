using ChatBotAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenAI;
using System.Text.Json;
using OpenAI.Chat;
using ChatBotAPI.Repositories;
using ChatBotAPI.Services;

namespace ChatBotAPI.Controllers
{
    public class ChatBotController : Controller
	{

		private readonly IMessageService _messageService;

		public ChatBotController(IMessageService messageService)
		{
			_messageService = messageService;

		}

		//create an endpoint to receive a message from the user and return a response from openAI for that message
		[HttpPost]
		[Route("api/chatbot/message")]
		public async Task<IActionResult> GetReply([FromBody] UserMessage message)
		{
            var reply = await _messageService.CompleteMessage(message);
			return Ok(reply);
		}


	 private string GetRelevantLocalData(string query)
        {
            // Path to the LocalData folder
            var localDataPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalData", "clientsTrainingData.json");

            if (!System.IO.File.Exists(localDataPath))
            {
                return "No relevant data found.";
            }

            // Read and parse the JSON file
            var jsonData = System.IO.File.ReadAllText(localDataPath);
            var trainingData = JsonSerializer.Deserialize<TrainingData[]>(jsonData);

            // Find the most relevant data (simple keyword matching for now)
            var relevantData = trainingData
                .Where(data => data.Prompt.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .Select(data => $"{data.Prompt} -> {data.Completion}")
                .ToList();

            return relevantData.Any() ? string.Join("\n", relevantData) : "No relevant data found.";
        }
    }

    // Model for training data
    public class TrainingData
    {
        public string Prompt { get; set; }
        public string Completion { get; set; }
    }
}
