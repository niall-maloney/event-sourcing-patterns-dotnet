using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.Dashboard.Clients;

public class MatchingClient : IMatchingClient
{
    private readonly HttpClient _httpClient;

    public MatchingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MatchingManager[]> GetManagers()
    {
        return await _httpClient.GetFromJsonAsync<MatchingManager[]>("/matching-managers") ??
               Array.Empty<MatchingManager>();
    }

    public async Task<Expectation[]> GetExpectations()
    {
        return await _httpClient.GetFromJsonAsync<Expectation[]>("/expectations") ??
               Array.Empty<Expectation>();
    }

    public async Task<Payment[]> GetPayments()
    {
        return await _httpClient.GetFromJsonAsync<Payment[]>("/payments") ??
               Array.Empty<Payment>();
    }

    public Task BeginMatching(MatchingDefinition definition)
    {
        return _httpClient.PostAsJsonAsync("/matching-managers", definition);
    }

    public Task CreateExpectation(ExpectationDefinition definition)
    {
        return _httpClient.PostAsJsonAsync("/expectations", definition);
    }

    public Task CreatePayment(PaymentDefinition definition)
    {
        return _httpClient.PostAsJsonAsync("/payments", definition);
    }
}
