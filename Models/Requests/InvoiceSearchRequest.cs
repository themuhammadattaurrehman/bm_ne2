namespace dotnet.Models
{
    public class InvoiceSearchRequest
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Search { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
