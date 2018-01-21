using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.UseCases
{
    public abstract class UseCaseBase : IUseCase
    {
        public abstract string Name { get; }

        protected T RequestInput<T>(string message, Func<string, InputValidationResult> inputValidator = null)
        {
            string input = RequestInput(message, inputValidator);

            return (T)Convert.ChangeType(input, typeof(T));
        }

        protected string RequestInput(string message, Func<string, InputValidationResult> inputValidator = null)
        {
            Console.WriteLine(message);
            // Get input from console.
            string input = Console.ReadLine();

            if(inputValidator != null)
            {
                while(true)
                {
                    // Validate input.
                    InputValidationResult result = inputValidator.Invoke(input);
                    if(!result.IsSuccessful)
                    {
                        // Display error.
                        Console.WriteLine("Input error:");
                        foreach(string error in result.ErrorMessages)
                        {
                            System.Console.WriteLine($"- {error}");
                        }

                        Console.WriteLine(message);
                        input = Console.ReadLine();
                    }
                    else
                    {
                        // Input is valid.
                        break;
                    }
                }
            }

            return input;
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));

        protected class InputValidationResult
        {
            public static readonly InputValidationResult Success = new InputValidationResult(true, string.Empty);

            public bool IsSuccessful { get; private set; }
            public string[] ErrorMessages { get; private set; }

            private InputValidationResult(bool isSuccessful, params string[] errorMessages)
            {
                IsSuccessful = isSuccessful;
                ErrorMessages = errorMessages;
            }

            public static InputValidationResult WithErrors(params string[] errorMessages)
            {
                return new InputValidationResult(false, errorMessages);
            }
        }
    }
}