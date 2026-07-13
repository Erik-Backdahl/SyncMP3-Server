public static class NetworkKeyGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private static readonly Random Rng = Random.Shared;
    public static string GenerateCode(int length = 6)
    {
        return new string(Enumerable.Range(0, length)
            .Select(_ => Alphabet[Rng.Next(Alphabet.Length)])
            .ToArray());
    }
}