namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

public record PaymentDefinition(string Iban, decimal Amount, string Reference);