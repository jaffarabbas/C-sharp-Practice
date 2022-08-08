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
        private readonly IProductRepository _productRepository;
        
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
