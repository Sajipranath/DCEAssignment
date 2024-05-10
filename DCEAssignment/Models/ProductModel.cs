namespace DCEAssignment.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int SupplierId { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
