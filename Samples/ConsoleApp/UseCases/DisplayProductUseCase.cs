using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using ReadSide.Products;
using ReadSide.Products.Repositories;

namespace ConsoleApp.UseCases
{
    public class DisplayProductUseCase : UseCaseBase
    {
        private readonly IProductReadSideRepository _productReadSideRepository;

        public override string Name => "DisplayProduct";

        public DisplayProductUseCase(IProductReadSideRepository productReadSideRepository)
        {
            _productReadSideRepository = productReadSideRepository;    
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int productId = RequestInput<int>("Enter ID of product to display:", input =>
            {
                if(int.TryParse(input, out int i))
                {
                    return InputValidationResult.Success;
                }

                return InputValidationResult.WithErrors("Invalid product ID.");
            });

            ProductReadModel product = await _productReadSideRepository.GetProductByIdAsync(productId);

            if(product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            Console.WriteLine($"Product ID: {product.ProductId}, Product Name: {product.ProductName}, IsActive: {product.IsActive}");
        }
    }
}