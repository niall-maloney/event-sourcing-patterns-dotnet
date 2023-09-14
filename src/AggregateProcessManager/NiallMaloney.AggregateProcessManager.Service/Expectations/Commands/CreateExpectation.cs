using MediatR;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Commands;

public record CreateExpectation(string ExpectationId, string Iban, decimal Amount, string Reference) : IRequest;
