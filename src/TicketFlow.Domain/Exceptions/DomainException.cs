using System.Diagnostics.CodeAnalysis;

namespace TicketFlow.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class DomainException(string message) : Exception(message);