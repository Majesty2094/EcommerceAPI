using MajesticEcommerceAPI.Models;
using MajesticEcommerceAPI.Data;
using MajesticEcommerceAPI.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MajesticEcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;


        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
public async Task<IActionResult> GetProducts(
    [FromQuery] int limit = 10,
    [FromQuery] int offset = 0,
    [FromQuery] string? search = null)
{
    if (limit <= 0 || offset < 0)
        return BadRequest("Limit > 0 and Offset >= 0.");

    // build filtered query
    var query = _context.Products.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(p => p.Name.Contains(search));

    // total items
    var totalItems = await query.CountAsync();

    // page of data
    var items = await query
        .OrderBy(p => p.Name)
        .Skip(offset)
        .Take(limit)
        .Select(p => new ProductDto {
            Id       = p.Id,
            Name     = p.Name,
            ImageUrl = p.ImageUrl,
            Price    = p.Price
        })
        .ToListAsync();

    // paging metadata
    var totalPages  = (int)Math.Ceiling((double)totalItems / limit);
    var currentPage = (offset / limit) + 1;
    var nextOffset  = offset + limit;
    var prevOffset  = offset - limit;

    return Ok(new {
        limit,
        offset,
        page        = currentPage,
        totalItems,
        totalPages,
        nextOffset  = nextOffset < totalItems ? (int?)nextOffset : null,
        prevOffset  = prevOffset >= 0          ? (int?)prevOffset : null,
        items
    });
}


        //  Add Product Admin 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto ProductDto)
        {

                _logger.LogInformation("AddProduct endpoint called by user: {User}", User.Identity?.Name);

            if (!ModelState.IsValid) return BadRequest(ModelState);
             

            string imagePath = null;
            if (ProductDto.Image !=null)
            {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProductDto.Image.FileName);
                var filePath = Path.Combine("Images", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await ProductDto.Image.CopyToAsync(stream);
        }
                 imagePath = $"{Request.Scheme}://{Request.Host}/Images/{fileName}";
            }    
            
            var product = new Product
            {
                Name = ProductDto.Name,
                Price = ProductDto.Price,
                ImageUrl = imagePath    
                
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();


            return Ok(new { message = "Product added successfully." });
        }

        //  Update Product Admin 
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found.");
             
            if (productDto.Image != null)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
        var filePath = Path.Combine("Images", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await productDto.Image.CopyToAsync(stream);
        }
        product.ImageUrl = $"{Request.Scheme}://{Request.Host}/Images/{fileName}";
    }

            product.Name = productDto.Name;
            product.Price = productDto.Price;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully." });
            
        }
    }
}
