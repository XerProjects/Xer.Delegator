using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Commands;
using Domain.DomainEvents;
using Xer.Delegator;

namespace ConsoleApp.UseCases
{
    public class RegisterProductUseCase : UseCaseBase
    {
        private readonly IMessageDelegator _commandDispatcher;

        public override string Name => "RegisterProduct";

        public RegisterProductUseCase(IMessageDelegator commandDispatcher)
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

            string productName = RequestInput("Enter product name:");

            await _commandDispatcher.SendAsync(new RegisterProductCommand(productId, productName));

            Console.WriteLine($"{productName} registered.");
        }
    }
}