using System.Collections.Generic;
using WebApiFromScratch.Models;

namespace WebApiFromScratch.Repository
{
    public interface IProductRepository
    {
        int AddProduct(ProductModel product);
        List<ProductModel> GetProducts();
    }
}