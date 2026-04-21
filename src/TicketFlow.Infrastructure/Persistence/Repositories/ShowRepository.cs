using Microsoft.EntityFrameworkCore;
using TicketFlow.Application.DTOs.Responses;
using TicketFlow.Application.UseCases.Shows;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Enums;
using TicketFlow.Domain.Repositories;

namespace TicketFlow.Infrastructure.Persistence.Repositories;

public class ShowRepository(TicketFlowDbContext context) : IShowRepository, IShowReadRepository
{
    public void Add(Show show)
    {
        context.Shows.Add(show);
    }

    public async Task<Show?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Shows.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ShowDetailsResponse?> GetOptimizedDetailsAsync(Guid showId, CancellationToken cancellationToken = default)
    {
        return await context.Shows.AsNoTracking()
            .Where(s => s.Id == showId)
            .Select(s => new ShowDetailsResponse(
                s.Id,
                s.Title,
                s.Date,
                s.Status.ToString(),
                context.Tickets.Count(t => t.ShowId == s.Id),
                context.Tickets.Count(t => t.ShowId == s.Id && t.Status == TicketStatus.Available)))
            .FirstOrDefaultAsync(cancellationToken);
    }
}