using Microsoft.AspNetCore.Mvc;
using TicketFlow.Application.UseCases.Tickets.Reserve;

namespace TicketFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class TicketsController : ControllerBase
{
    [HttpPost("{id:guid}/reserve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reserve(IReserveTicketUseCase useCase, Guid id, Guid customerId)
    {
        await useCase.ExecuteAsync(id, customerId);

        return NoContent();
    }
}