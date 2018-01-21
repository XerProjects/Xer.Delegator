using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Repositories;

namespace Domain.Commands
{
    public class ActivateProductCommand : ICommand
    {
        public int ProductId { get; }

        public ActivateProductCommand(int productId) 
        {
            ProductId = productId; 
        }
    }

    public class ActivateProductCommandHandler
    {
        private readonly IProductRepository _productRepository;

        public ActivateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        
        public async Task HandleActivateProductCommandAsync(ActivateProductCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            Product product = await _productRepository.GetProductByIdAsync(command.ProductId);
            if(product == null)
            {
                throw new ProductNotFoundException("Product not found.");
            }

            product.Activate();

            await _productRepository.SaveAsync(product);
        }
    }
}