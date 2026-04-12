using Microsoft.Extensions.DependencyInjection;
using TicketFlow.Application.UseCases.Tickets.Reserve;
using TicketFlow.Domain.Entities;
using TicketFlow.Domain.Exceptions;
using TicketFlow.Domain.ValueObjects;
using TicketFlow.Infrastructure.Persistence;
using TicketFlow.IntegrationTests.Abstractions;

namespace TicketFlow.IntegrationTests.UseCases.Tickets.Reserve;

[Collection(nameof(IntegrationTestCollection))]
[Trait("Category", "Integration")]
public class ReserveTicketUseCaseConcurrencyTests(IntegrationTestWebAppFactory factory)
{
    private readonly IServiceScopeFactory _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();

    [Fact(DisplayName = "Should prevent double reservation when two requests occur simultaneously")]
    public async Task Handle_ShouldPreventDoubleReservation_WhenConcurrentRequestsOccur()
    {
        // Arrange
        Guid ticketId;
        Guid userA = Guid.NewGuid();
        Guid userB = Guid.NewGuid();

        DateTime now = DateTime.UtcNow;

        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TicketFlowDbContext>();

            Show show = new(title: "Rock in Rio", date: now.AddMonths(1), maxTicketsPerUser: 10, currentDate: now);
            Ticket ticket = new(show.Id, new Seat(Sector: "General", Row: "1", Number: "1"), price: 100m, createdDate: now);

            context.Shows.Add(show);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            ticketId = ticket.Id;
        }

        // Act
        var taskA = Task.Run<(bool Success, Exception? Exception)>(async () =>
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<IReserveTicketUseCase>();

            try
            {
                await useCase.ExecuteAsync(ticketId, userA, CancellationToken.None);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        });

        var taskB = Task.Run<(bool Success, Exception? Exception)>(async () =>
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<IReserveTicketUseCase>();

            try
            {
                await useCase.ExecuteAsync(ticketId, userB, CancellationToken.None);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        });

        var results = await Task.WhenAll(taskA, taskB);

        // Assert
        var successes = results.Count(r => r.Success);
        var failures = results.Count(r => !r.Success);
        
        Assert.Equal(1, successes);
        Assert.Equal(1, failures);
        
        var failureResult = results.First(r => !r.Success);
        Assert.IsType<ConcurrencyException>(failureResult.Exception);
    }
}