namespace MajesticEcommerceAPI.DTOs.Transaction
{
    public class CreateTransactionDto
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
