using MediatR;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Commands;

public record CreateExpectation(string ExpectationId, string Iban, decimal Amount, string Reference)
    : IRequest;