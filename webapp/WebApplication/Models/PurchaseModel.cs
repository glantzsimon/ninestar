namespace K9.WebApplication.Models
{
    public class PurchaseModel
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int ContactId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmailAddress { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}