namespace MajesticEcommerceAPI.DTOs.Transaction
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Reference{ get; set; }
        public DateTime Date { get; set; }
    }

    
}
