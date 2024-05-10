namespace DCEAssignment.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public byte OrderStatus { get; set; }
        public byte OrderType { get; set; }
        public int OrderById { get; set; }
        public DateTime OrderedOn { get; set; }
        public DateTime ShippedOn { get; set; }
        public bool IsActive { get; set; } = true;

        // From Customer table
        public string Username { get; set; } = "";

        // From Product table
        public decimal UnitPrice { get; set; } 
        public string ProductName { get; set; }= "";
    }
}
