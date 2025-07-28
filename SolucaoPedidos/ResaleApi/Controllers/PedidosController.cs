using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResaleApi.Features.Commands;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PedidosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("receber-cliente")]
    public async Task<IActionResult> ReceberPedidoCliente([FromBody] ReceberPedidoClienteCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}