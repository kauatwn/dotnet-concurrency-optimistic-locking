using System.Diagnostics.CodeAnalysis;

namespace TicketFlow.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class DomainConflictException(string message) : DomainException(message);