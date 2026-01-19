using Microsoft.AspNetCore.Mvc;
using DotNetAI.Application.Orchestrators;
using DotNetAI.Api.Models;

namespace DotNetAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly AiOrchestrator _orchestrator;

    public AiController(AiOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpPost]
    public ActionResult<AiResponseDto> Ask(AiRequestDto request)
    {
        var result = _orchestrator.Handle(request.Prompt);

        return Ok(new AiResponseDto
        {
            Content = result.Content
        });
    }
}
