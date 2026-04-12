using System.Diagnostics.CodeAnalysis;
using TicketFlow.Application.DTOs.Responses;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Repositories;

namespace TicketFlow.Application.UseCases.Tickets.GetAvailable;

[ExcludeFromCodeCoverage(Justification = "Read-only orchestration intentionally excluded. Focus is strictly on Write concurrency and Optimistic Locking.")]
public sealed class GetAvailableTicketsUseCase(ITicketRepository ticketRepository) : IGetAvailableTicketsUseCase
{
    public async Task<List<TicketResponse>> ExecuteAsync(
        Guid showId,
        CancellationToken cancellationToken = default)
    {
        List<Ticket> tickets = await ticketRepository.GetAvailableAsync(showId, cancellationToken);

        return tickets.Select(t => new TicketResponse(
            t.Id,
            t.Seat.Sector,
            t.Seat.Row,
            t.Seat.Number,
            t.Price
        )).ToList();
    }
}