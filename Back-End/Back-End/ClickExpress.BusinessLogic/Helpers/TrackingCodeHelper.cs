namespace ClickExpress.BusinessLogic.Helpers
{
    public static class TrackingCodeHelper
    {
        private static readonly Random _rng = Random.Shared;

        public static string Generate()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var suffix = _rng.Next(1000, 9999);
            return $"CE-{date}-{suffix}";
        }

        public static bool IsValid(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            var parts = code.Split('-');
            return parts.Length == 3
                && parts[0] == "CE"
                && parts[1].Length == 8
                && parts[1].All(char.IsDigit)
                && parts[2].Length == 4
                && parts[2].All(char.IsDigit);
        }
    }
}
