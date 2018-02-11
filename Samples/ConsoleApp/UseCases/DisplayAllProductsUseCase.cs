using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadSide.Products;
using ReadSide.Products.Repositories;

namespace ConsoleApp.UseCases
{
    public class DisplayAllProductsUseCase : UseCaseBase
    {
        public override string Name => "DisplayAllProducts";
        private readonly IProductReadSideRepository _productReadSideRepository;

        public DisplayAllProductsUseCase(IProductReadSideRepository productReadSideRepository)
        {
            _productReadSideRepository = productReadSideRepository;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IReadOnlyCollection<ProductReadModel> products = await _productReadSideRepository.GetAllProductsAsync(cancellationToken);
            
            foreach(ProductReadModel product in products)
                Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}, IsActive: {product.IsActive}");
        }
    }
}