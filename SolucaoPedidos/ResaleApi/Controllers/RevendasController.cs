using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResaleApi.Features.Commands;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class RevendasController : ControllerBase
{
    private readonly IMediator _mediator;

    public RevendasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriarRevendaCommand command)
    {
        var revendaId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = revendaId }, command);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        // Adicionar query para buscar revenda por ID
        return Ok(); // Implementar
    }
}