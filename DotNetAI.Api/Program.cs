using DotNetAI.AI.Abstractions;
using DotNetAI.Application.Orchestrators;
using DotNetAI.Infrastructure.AiClients;
using DotNetAI.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI + Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind OpenAI options from configuration
builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection("OpenAI")
);

// HttpClient for OpenAI
builder.Services.AddHttpClient<IAiClient, OpenAiClient>();

// Application services
builder.Services.AddScoped<AiOrchestrator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
