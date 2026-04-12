using System.Diagnostics.CodeAnalysis;

namespace TicketFlow.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class ConcurrencyException(string message) : Exception(message);