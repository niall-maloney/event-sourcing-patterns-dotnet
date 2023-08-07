using System.Text;
using Nito.Guids;

namespace NiallMaloney.Shared;

public static class DeterministicIdFactory
{
    private static readonly Guid Namespace = new("8f582e2e-eab8-4c2e-a936-97f668662a88");

    public static string NewId(params string[] components) =>
        !components.Any()
            ? GuidFactory.CreateRandom().ToString()
            : GuidFactory.CreateSha1(Namespace, Encoding.UTF8.GetBytes(string.Join('-', components))).ToString();
}