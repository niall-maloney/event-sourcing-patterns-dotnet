using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.Dashboard.Clients;

public interface IMatchingClient
{
    public Task<MatchingManager[]> GetManagers();
    public Task<Expectation[]> GetExpectations();
    public Task<Payment[]> GetPayments();
    public Task BeginMatching(MatchingDefinition definition);
    public Task CreateExpectation(ExpectationDefinition definition);
    public Task CreatePayment(PaymentDefinition definition);
}
