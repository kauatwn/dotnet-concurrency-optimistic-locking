using System.Diagnostics.CodeAnalysis;

namespace TicketFlow.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException(string message) : Exception(message);