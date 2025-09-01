namespace MajesticEcommerceAPI.DTOs.Transaction
{
    public class TransactionWithUserDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }  // Include user's email
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Reference {get; set; }
        public DateTime Date { get; set; }

    }
}
