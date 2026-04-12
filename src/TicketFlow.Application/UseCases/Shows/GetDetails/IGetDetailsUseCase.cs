using TicketFlow.Application.DTOs.Responses;

namespace TicketFlow.Application.UseCases.Shows.GetDetails;

public interface IGetDetailsUseCase
{
    Task<ShowDetailsResponse> ExecuteAsync(Guid showId, CancellationToken cancellationToken = default);
}