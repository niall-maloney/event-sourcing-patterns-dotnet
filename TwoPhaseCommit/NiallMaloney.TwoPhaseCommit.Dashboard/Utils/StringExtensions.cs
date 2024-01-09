namespace NiallMaloney.TwoPhaseCommit.Dashboard.Utils;

public static class StringExtensions
{
    public static string Truncate(this string str, int length = 8) => $"{str[..length]}...";
}
