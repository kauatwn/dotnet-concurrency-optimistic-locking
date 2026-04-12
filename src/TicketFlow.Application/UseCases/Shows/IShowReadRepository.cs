using TicketFlow.Application.DTOs.Responses;

namespace TicketFlow.Application.UseCases.Shows;

public interface IShowReadRepository
{
    Task<ShowDetailsResponse?> GetOptimizedDetailsAsync(Guid showId, CancellationToken cancellationToken);
}