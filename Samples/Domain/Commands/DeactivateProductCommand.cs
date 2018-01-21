using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Repositories;

namespace Domain.Commands
{
    public class DeactivateProductCommand : ICommand
    {
        public int ProductId { get; }

        public DeactivateProductCommand(int productId) 
        {
            ProductId = productId; 
        }        
    }

    public class DeactivateProductCommandHandler
    {
        private readonly IProductRepository _productRepository;

        public DeactivateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task HandleDeactivateProductCommandAsync(DeactivateProductCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            Product product = await _productRepository.GetProductByIdAsync(command.ProductId);
            if(product == null)
            {
                throw new ProductNotFoundException("Product not found.");
            }

            product.Deactivate();

            await _productRepository.SaveAsync(product);
        }
    }
}