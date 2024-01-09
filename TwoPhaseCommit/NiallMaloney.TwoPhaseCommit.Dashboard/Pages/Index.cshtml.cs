using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NiallMaloney.TwoPhaseCommit.Dashboard.Clients;
using NiallMaloney.TwoPhaseCommit.Service.Expectations.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;
using NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

namespace NiallMaloney.TwoPhaseCommit.Dashboard.Pages;

public class IndexModel : PageModel
{
    private const string Iban = "GB33BUKB20201555555555";
    private const decimal Amount = 100;
    private const string Reference = "prt_xyz";

    private readonly ILogger<IndexModel> _logger;
    private readonly IMatchingClient _client;

    public Expectation[] Expectations = Array.Empty<Expectation>();
    public MatchingManager[] Managers = Array.Empty<MatchingManager>();
    public Payment[] Payments = Array.Empty<Payment>();

    public IndexModel(ILogger<IndexModel> logger, IMatchingClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task OnGet()
    {
        var getManagers = _client.GetManagers();
        var getPayments = _client.GetPayments();
        var getExpectations = _client.GetExpectations();
        await Task.WhenAll(getManagers, getPayments, getExpectations);

        Managers = getManagers.Result;
        Payments = getPayments.Result;
        Expectations = getExpectations.Result;
    }

    public async Task<IActionResult> OnPostExpectation()
    {
        _logger.LogInformation("Creating a new expectation.");

        await _client.CreateExpectation(new ExpectationDefinition(Iban, Amount, Reference));
        return RedirectToIndexResult();
    }

    public async Task<IActionResult> OnPostPayment()
    {
        _logger.LogInformation("Creating a new payment.");

        await _client.CreatePayment(new PaymentDefinition(Iban, Amount, Reference));
        return RedirectToIndexResult();
    }

    public async Task<IActionResult> OnPostMatch(string paymentId, string expectationId)
    {
        _logger.LogInformation("Starting to match payment '{PaymentId}' to expectation '{ExpectationId}'.", paymentId,
            expectationId);

        await _client.BeginMatching(new MatchingDefinition(ExpectationId: expectationId, PaymentId: paymentId,
            Iban: Iban, Amount: Amount, Reference: Reference));
        return RedirectToIndexResult();
    }

    private static RedirectToPageResult RedirectToIndexResult() => new("Index");
}
