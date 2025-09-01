using Microsoft.AspNetCore.Mvc;
using MajesticEcommerceAPI.Models;
using MajesticEcommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MajesticEcommerceAPI.DTOs.Transaction;

namespace MajesticEcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("all-transactions")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and pageSize must be greater than 0");

            var transactions = await _context.Transactions
                .OrderByDescending(t => t.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionWithUserDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    UserEmail = t.User.Email,
                    Description = t.Description,
                    Amount = t.Amount,
                    Reference = t.Reference,
                    Date = t.Date
                })
                .ToListAsync();
                var search = string.Empty;
            var newTransact = transactions.Where(x => x.Description.Contains(search)).ToList();

            return Ok(transactions);
        }
    }
}
