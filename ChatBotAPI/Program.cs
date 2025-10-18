using Amazon.SQS;
using ChatBotAPI;
using ChatBotAPI.Helpers;
using ChatBotAPI.Models;
using ChatBotAPI.Repositories;
using ChatBotAPI.Services;
using ChatBotAPI.Services.BackgroundServices;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json")
		.Build())
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<List<ClientDataConfig>>(builder.Configuration.GetSection("ClientsApplicationData"));
builder.Services.Configure<SqsSettings>(builder.Configuration.GetSection("SqsSettings"));
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IRiskManagerService, RiskManagerService>();
builder.Services.AddScoped<IRiskManagerRepo, RiskManagerRepo>();
builder.Services.AddSingleton<ChatContext>();
builder.Services.AddSingleton<Helper>();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowOrigin",
		builder =>
		{
			builder.WithOrigins("http://localhost:3000")
				   .AllowAnyHeader()
				   .AllowAnyMethod();
		});
});

// Configure AWS SQS Client
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
	var sqsSettings = builder.Configuration.GetSection("SqsSettings").Get<SqsSettings>();
	
	if (!string.IsNullOrEmpty(sqsSettings?.AwsAccessKeyId) && !string.IsNullOrEmpty(sqsSettings?.AwsSecretAccessKey))
	{
		// Use explicit credentials
		return new AmazonSQSClient(
			sqsSettings.AwsAccessKeyId,
			sqsSettings.AwsSecretAccessKey,
			Amazon.RegionEndpoint.GetBySystemName(sqsSettings.AwsRegion));
	}
	else
	{
		// Use default credentials (IAM role, environment variables, or AWS profile)
		return new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(sqsSettings?.AwsRegion ?? "us-west-2"));
	}
});

// Register the SQS Polling Background Service
builder.Services.AddHostedService<SqsPollingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(x => { x.AllowAnyHeader();x.AllowAnyMethod();x.AllowAnyOrigin(); });

app.UseAuthorization();

app.MapControllers();

try
{
	Log.Information("Starting ChatBot API application");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}
