using DotNetAI.Application.Orchestrators;
using DotNetAI.Infrastructure.AiClients;

Console.WriteLine("DotNetAI");
Console.Write("Ask something: ");

var input = Console.ReadLine();

var aiClient = new OpenAiClient();
var orchestrator = new AiOrchestrator(aiClient);

var response = orchestrator.Handle(input ?? string.Empty);

Console.WriteLine();
Console.WriteLine("Response:");
Console.WriteLine(response.Content);
