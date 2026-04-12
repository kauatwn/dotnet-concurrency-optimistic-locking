using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using TicketFlow.Application.UseCases.Shows.GetDetails;
using TicketFlow.Application.UseCases.Tickets.GetAvailable;
using TicketFlow.Application.UseCases.Tickets.Reserve;

namespace TicketFlow.Application.Extensions;

[ExcludeFromCodeCoverage(Justification = "Pure dependency injection configuration")]
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetAvailableTicketsUseCase, GetAvailableTicketsUseCase>();
        services.AddScoped<IReserveTicketUseCase, ReserveTicketUseCase>();
        
        services.AddScoped<IGetDetailsUseCase, GetDetailsUseCase>();

        return services;
    }
}