using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using CQRSlite.Commands;
using CQRSlite.Routing;
using MediatR;
using Xer.Delegator.Registrations;

namespace Xer.Delegator.Benchmarks
{
    [MemoryDiagnoser]
    public class SingleMessageHandlerBenchmarks
    {
        #region Declarations

        private IMessageDelegator _delegator;
        private IMediator _mediator;
        private ICommandSender _commandSender;

        #endregion Declarations

        #region Setup
        
        [GlobalSetup]
        public void Setup()
        {        
            // Delegator.
            var reg = new SingleMessageHandlerRegistration();
            reg.Register<TestMessage>(handleMessage);
            _delegator = new MessageDelegator(reg.BuildMessageHandlerResolver());

            // Mediatr.
            var requestHandler = new TestMediatrRequestHandler();
            _mediator = new Mediator((t) => requestHandler, (t) => Enumerable.Empty<object>());

            // CQRSLite.
            Router router = new Router();
            router.RegisterHandler<TestCqrsliteCommand>(handleCqrsliteCommand);
            _commandSender = router;
        }
        
        #endregion Setup
        
        #region Benchmarks

        [Benchmark]
        public Task BenchmarkDelegator() => _delegator.SendAsync(new TestMessage());

        [Benchmark]
        public Task BenchmarkCqrsLite() => _commandSender.Send(new TestCqrsliteCommand());

        [Benchmark]
        public Task BenchmarkMediatr() => _mediator.Send(new TestMediatrRequest());

        #endregion Benchmarks

        private Task handleMessage(TestMessage message, CancellationToken ct) => Task.CompletedTask;

        private Task handleCqrsliteCommand(TestCqrsliteCommand message, CancellationToken ct) => Task.CompletedTask;

        class TestCqrsliteCommand : ICommand
        {
            public int ExpectedVersion { get; }
        }

        class TestMediatrRequest : IRequest { }

        class TestMediatrRequestHandler : IRequestHandler<TestMediatrRequest>
        {
            public Task Handle(TestMediatrRequest message, CancellationToken cancellationToken) => Task.CompletedTask;
        }
    }
}