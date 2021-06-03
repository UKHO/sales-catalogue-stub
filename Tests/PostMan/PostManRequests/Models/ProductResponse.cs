using System.Collections.Generic;
using System.Net;

namespace PostManRequests.Models
{
    public class Cancellation
    {
        public int editionNumber { get; set; }
        public int updateNumber { get; set; }
    }

    public class Product
    {
        public string productName { get; set; }
        public int editionNumber { get; set; }
        public List<int> updateNumbers { get; set; }
        public int fileSize { get; set; }
        public Cancellation cancellation { get; set; }
    }

    public class ProductCounts
    {
        public int requestedProductCount { get; set; }
        public int returnedProductCount { get; set; }
        public int requestedProductsAlreadyUpToDateCount { get; set; }
        public List<object> requestedProductsNotReturned { get; set; }
    }

    public class ProductResponse
    {
        public List<Product> products { get; set; }
        public ProductCounts productCounts { get; set; }
    }

  
}