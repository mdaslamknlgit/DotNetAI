using Microsoft.AspNetCore.Mvc;
using DotNetAI.Application.Orchestrators;
using DotNetAI.Infrastructure.AiClients;
using DotNetAI.Api.Models;

namespace DotNetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    [HttpPost]
    public ActionResult<AiResponseDto> Ask(AiRequestDto request)
    {
        var aiClient = new OpenAiClient();
        var orchestrator = new AiOrchestrator(aiClient);

        var result = orchestrator.Handle(request.Prompt);

        return Ok(new AiResponseDto
        {
            Content = result.Content
        });
    }
}
