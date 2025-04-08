using ChatBotAPI.Helpers;
using ChatBotAPI.Models;
using ChatBotAPI.Repositories;
using ChatBotAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<List<ClientDataConfig>>(builder.Configuration.GetSection("ClientsApplicationData"));
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

app.Run();
