namespace TicketFlow.Application.UseCases.Tickets.Reserve;

public interface IReserveTicketUseCase
{
    Task ExecuteAsync(Guid ticketId, Guid customerId, CancellationToken cancellationToken = default);
}