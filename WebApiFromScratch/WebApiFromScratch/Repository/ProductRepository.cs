
using System.Collections.Generic;
using WebApiFromScratch.Models;

namespace WebApiFromScratch.Repository
{
    public class ProductRepository
    {
        private List<ProductModel> list = new List<ProductModel>();
        public int AddProduct(ProductModel product)
        {
            product.Id = list.Count + 1;
            list.Add(product);
            return product.Id;
        }

        public List<ProductModel> GetProducts()
        {
            return list;
        }
    }
}
