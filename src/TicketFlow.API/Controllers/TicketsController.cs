using Microsoft.AspNetCore.Mvc;
using TicketFlow.API.Contracts.Tickets;
using TicketFlow.Application.UseCases.Tickets.Reserve;

namespace TicketFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController(IReserveTicketUseCase reserveTicketUseCase) : ControllerBase
{
    [HttpPost("{id:guid}/reserve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reserve(Guid id, ReserveTicketRequest request)
    {
        await reserveTicketUseCase.ExecuteAsync(id, request.CustomerId);

        return NoContent();
    }
}