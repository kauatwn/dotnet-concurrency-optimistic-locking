using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Exceptions;
using TicketFlow.Domain.Repositories;

namespace TicketFlow.Application.UseCases.Tickets.Reserve;

public sealed class ReserveTicketUseCase(
    ITicketRepository ticketRepository,
    IShowRepository showRepository,
    TimeProvider timeProvider) : IReserveTicketUseCase
{
    public async Task ExecuteAsync(Guid ticketId, Guid customerId, CancellationToken cancellationToken = default)
    {
        Ticket ticket = await ticketRepository.GetByIdAsync(ticketId, cancellationToken)
                        ?? throw new NotFoundException($"Ticket {ticketId} not found.");

        Show show = await showRepository.GetByIdAsync(ticket.ShowId, cancellationToken)
                    ?? throw new NotFoundException($"Show with ID '{ticket.ShowId}' not found.");

        DateTime now = timeProvider.GetUtcNow().UtcDateTime;

        if (!show.CanSellTickets(now))
        {
            throw new DomainConflictException("Cannot reserve ticket. The show is unavailable or finished.");
        }

        ticket.Reserve(customerId, now);
        await ticketRepository.UpdateAsync(ticket, cancellationToken);
    }
}