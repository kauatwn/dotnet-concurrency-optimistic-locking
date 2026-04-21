using Microsoft.EntityFrameworkCore;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Enums;
using TicketFlow.Domain.Exceptions;
using TicketFlow.Domain.Repositories;

namespace TicketFlow.Infrastructure.Persistence.Repositories;

public class TicketRepository(TicketFlowDbContext context) : ITicketRepository
{
    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Ticket>> GetAvailableAsync(Guid showId, CancellationToken cancellationToken = default)
    {
        return await context.Tickets
            .AsNoTracking()
            .Where(t => t.ShowId == showId && t.Status == TicketStatus.Available)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        context.Tickets.Update(ticket);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                "The data was modified by another user while you were trying to save. Please refresh and try again.");
        }
    }
}