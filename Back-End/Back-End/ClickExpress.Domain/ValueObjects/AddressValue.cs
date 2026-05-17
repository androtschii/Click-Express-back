namespace ClickExpress.Domain.ValueObjects
{
    public class AddressValue
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;

        public override string ToString() => $"{Street}, {City}, {State} {ZipCode}";
    }
}
