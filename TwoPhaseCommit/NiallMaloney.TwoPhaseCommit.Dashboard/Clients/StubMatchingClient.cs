using System.Security.Cryptography.Xml;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.Dashboard.Clients;

public class StubMatchingClient : IMatchingClient
{
    public Task<MatchingManager[]> GetManagers() =>
        Task.FromResult(new MatchingManager[]
        {
            new(Id: "0be9b7f8-e9a7-404a-91ef-367bb55bbc0c", PaymentId: "24f24379-bfb0-4cb4-9f3f-e3a62c44d358",
                ExpectationId: "4556db29-9ab0-4c08-9d44-bc0529c7050f", Iban: "AN_IBAN", Amount: 100, Reference: "stub",
                Status: "Created", 1)
        });

    public Task<Expectation[]> GetExpectations() =>
        Task.FromResult(new Expectation[]
        {
            new(Id: "4556db29-9ab0-4c08-9d44-bc0529c7050f", Iban: "AN_IBAN", Amount: 100, Reference: "stub",
                Status: "Created", 1),
            new(Id: "a606b6b8-5faa-4a38-b00f-560c4519ff9b", Iban: "AN_IBAN", Amount: 100, Reference: "stub",
                Status: "Created", 1)
        });

    public Task<Payment[]> GetPayments() =>
        Task.FromResult(new Payment[]
        {
            new(Id: "24f24379-bfb0-4cb4-9f3f-e3a62c44d358", Iban: "AN_IBAN", Amount: 100, Reference: "stub",
                Status: "Created", 1),
            new(Id: "24086043-7c26-4c19-a5cb-ed2dfd9cbdba", Iban: "AN_IBAN", Amount: 100, Reference: "stub",
            Status: "Created", 1)
        });

    public Task BeginMatching(MatchingDefinition definition) => throw new NotImplementedException();

    public Task CreateExpectation(ExpectationDefinition definition) => throw new NotImplementedException();
    public Task CreatePayment(PaymentDefinition definition) => throw new NotImplementedException();
}
