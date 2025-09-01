using Microsoft.AspNetCore.Mvc;
using MajesticEcommerceAPI.Models;
using MajesticEcommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MajesticEcommerceAPI.DTOs.Transaction;

namespace MajesticEcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim not found in token");

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized("Neither user ID nor email found in token");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user == null)
                    return Unauthorized("User not found");

                userId = user.Id;
            }

            var transaction = new Transaction
            {
                UserId = userId,
                Description = dto.Description,
                Amount = dto.Amount,
                Reference = dto.Reference,
                Date = DateTime.Now,
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Transaction created successfully",
                transactionId = transaction.Id
            });
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be greater than 0");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim not found in token");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized("Invalid user ID in token");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return Unauthorized("User not found");

            List<Transaction> transactions;

            if (user.Role.Equals("Admin"))
            {
                transactions = await _context.Transactions
                    .OrderByDescending(t => t.Date)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                transactions = await _context.Transactions
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Description = t.Description,
                Amount = t.Amount,
                Reference = t.Reference,
                Date = t.Date
            });

            var totalCount = user.Role.Equals("Admin") 
    ? await _context.Transactions.CountAsync()
    : await _context.Transactions.CountAsync(t => t.UserId == userId);
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

return Ok(new
{
    page,
    pageSize,
    totalCount,
    totalPages,
    nextPage = page < totalPages ? (int?)(page + 1) : null,
    prevPage = page > 1         ? (int?)(page - 1): null,
    transactions = transactionDtos
});
           
        }
    }
}
