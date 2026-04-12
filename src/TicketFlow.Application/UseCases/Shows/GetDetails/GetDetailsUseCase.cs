using System.Diagnostics.CodeAnalysis;
using TicketFlow.Application.DTOs.Responses;
using TicketFlow.Domain.Exceptions;

namespace TicketFlow.Application.UseCases.Shows.GetDetails;

[ExcludeFromCodeCoverage(Justification = "Read-only orchestration intentionally excluded. Focus is strictly on Write concurrency and Optimistic Locking.")]
public sealed class GetDetailsUseCase(IShowReadRepository showReadRepository) : IGetDetailsUseCase
{
    public async Task<ShowDetailsResponse> ExecuteAsync(Guid showId, CancellationToken cancellationToken = default)
    {
        ShowDetailsResponse? showDetails = await showReadRepository.GetOptimizedDetailsAsync(showId, cancellationToken);

        return showDetails ?? throw new NotFoundException($"Show with ID '{showId}' not found.");
    }
}