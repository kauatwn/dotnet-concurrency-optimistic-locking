using TicketFlow.Application.DTOs.Responses;

namespace TicketFlow.Application.UseCases.Tickets.GetAvailable;

public interface IGetAvailableTicketsUseCase
{
    Task<List<TicketResponse>> ExecuteAsync(Guid showId, CancellationToken cancellationToken = default);
}