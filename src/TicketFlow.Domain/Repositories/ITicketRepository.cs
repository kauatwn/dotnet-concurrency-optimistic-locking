using TicketFlow.Domain.Entities;

namespace TicketFlow.Domain.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Ticket>> GetAvailableAsync(Guid showId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken);
}