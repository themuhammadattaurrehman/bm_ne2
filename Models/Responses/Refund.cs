namespace dotnet.Models
{
    public class Refund
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }

        public int RefundAmount { get; set; }
        public int FinalAmount { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual Receipt Receipt { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
