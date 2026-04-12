namespace TicketFlow.Application.DTOs.Responses;

public sealed record TicketResponse(Guid Id, string Sector, string Row, string Number, decimal Price);