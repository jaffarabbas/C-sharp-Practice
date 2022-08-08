using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiFromScratch.Models;
using WebApiFromScratch.Repository;

namespace WebApiFromScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        
        public ProductController()
        {
            _productRepository = new ProductRepository();
        }
        
        [HttpPost("")]
        public IActionResult AddProducts(ProductModel product)
        {
            _productRepository.AddProduct(product);
            var products = _productRepository.GetProducts();
            return Ok(products);
        }
    }
}
