using TicketFlow.Application.DTOs.Responses;
using TicketFlow.Domain.Exceptions;

namespace TicketFlow.Application.UseCases.Shows.GetDetails;

public sealed class GetDetailsUseCase(IShowReadRepository showReadRepository) : IGetDetailsUseCase
{
    public async Task<ShowDetailsResponse> ExecuteAsync(Guid showId, CancellationToken cancellationToken = default)
    {
        ShowDetailsResponse? showDetails = await showReadRepository.GetOptimizedDetailsAsync(showId, cancellationToken);

        return showDetails ?? throw new NotFoundException($"Show with ID '{showId}' not found.");
    }
}