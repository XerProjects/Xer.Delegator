using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Commands;
using Xer.Delegator;

namespace ConsoleApp.UseCases
{
    public class ActivateProductUseCase : UseCaseBase
    {
        private readonly IMessageDelegator _commandDispatcher;

        public override string Name => "ActivateProduct";

        public ActivateProductUseCase(IMessageDelegator commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;    
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int productId = RequestInput<int>("Enter ID of product to activate:", input =>
            {
                if(int.TryParse(input, out int i))
                {
                    return InputValidationResult.Success;
                }

                return InputValidationResult.WithErrors("Invalid product ID.");
            });

            await _commandDispatcher.SendAsync(new ActivateProductCommand(productId));

            Console.WriteLine("Product activated.");
        }
    }
}