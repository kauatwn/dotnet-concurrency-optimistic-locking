using Microsoft.AspNetCore.Mvc;
using TicketFlow.Application.DTOs.Responses;
using TicketFlow.Application.UseCases.Shows.GetDetails;
using TicketFlow.Application.UseCases.Tickets.GetAvailable;

namespace TicketFlow.API.Controllers;

[Route("api/[controller]/{id:guid}")]
[ApiController]
public sealed class ShowsController : ControllerBase
{
    [HttpGet("details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShowDetailsResponse>> GetShowDetails(IGetDetailsUseCase useCase, Guid id)
    {
        ShowDetailsResponse show = await useCase.ExecuteAsync(id);

        return Ok(show);
    }

    [HttpGet("tickets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TicketResponse>>> GetAvailableTickets(IGetAvailableTicketsUseCase useCase, Guid id)
    {
        List<TicketResponse> tickets = await useCase.ExecuteAsync(id);

        return Ok(tickets);
    }
}