namespace MajesticEcommerceAPI.DTOs.Product{

    public class UpdateProductDto{
        public string Name { get; set; }
    public IFormFile Image { get; set; }
    public decimal Price { get; set; }
    
    public string cc {get; set;}
    }
}