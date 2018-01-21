using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Commands;
using Xer.Delegator;

namespace ConsoleApp.UseCases
{
    public class DeactivateProductUseCase : UseCaseBase
    {
        private readonly IMessageDelegator _commandDispatcher;

        public override string Name => "DeactivateProduct";

        public DeactivateProductUseCase(IMessageDelegator commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;    
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int productId = RequestInput<int>("Enter product ID:", input =>
            {
                if(int.TryParse(input, out int i))
                {
                    return InputValidationResult.Success;
                }

                return InputValidationResult.WithErrors("Invalid product ID.");
            });
            
            await _commandDispatcher.SendAsync(new DeactivateProductCommand(productId));

            Console.WriteLine("Product deactivated.");
        }
    }
}