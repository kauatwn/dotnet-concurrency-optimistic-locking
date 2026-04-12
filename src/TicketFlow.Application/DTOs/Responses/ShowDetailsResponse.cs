namespace TicketFlow.Application.DTOs.Responses;

public sealed record ShowDetailsResponse(
    Guid Id,
    string Title,
    DateTime Date,
    string Status,
    int TotalTickets,
    int AvailableTickets);