using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Repositories;

namespace Domain.Commands
{
    public class RegisterProductCommand : ICommand
    {
        public int ProductId { get; }
        public string ProductName { get; }
        
        public RegisterProductCommand(int productId, string productName) 
        {
            ProductId = productId;
            ProductName = productName;
        }
    }

    public class RegisterProductCommandHandler
    {
        private readonly IProductRepository _productRepository;

        public RegisterProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task HandleRegisterProductCommandAsync(RegisterProductCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _productRepository.SaveAsync(new Product(command.ProductId, command.ProductName));
        }
    }
}