using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using CQRSlite.Events;
using CQRSlite.Routing;
using MediatR;
using Xer.Delegator.Registrations;

namespace Xer.Delegator.Benchmarks
{
    [MemoryDiagnoser]
    public class MultiMessageHandlerBenchmarks
    {
        #region Declarations

        private IMessageDelegator _delegator;
        private IMediator _mediator;
        private IEventPublisher _eventPublisher;

        #endregion Declarations

        #region Setup

        [GlobalSetup]

        public void Setup()
        {            
            int numOfIterations = 100000;

            var reg = new MultiMessageHandlerRegistration();
            var router = new Router();
            var mediatrHandlers = new List<INotificationHandler<TestMediatrNotif>>();

            for (int i = 0; i < numOfIterations; i++)
            {
                // Register handlers N number of times each.
                reg.Register<TestMessage>(handleMessage);
                router.RegisterHandler<TestCqrsliteEvent>(handleCqrsliteMessage);
                mediatrHandlers.Add(new TestMediatrNotifHandler());
            }

            // Delegator.
            _delegator = new MessageDelegator(reg.BuildMessageHandlerResolver());
            // CQRSLite
            _eventPublisher = router;
            // Mediatr
            _mediator = new Mediator((t) => new object(), (t) => mediatrHandlers);
        }  

        #endregion Setup

        #region Benchmarks

        [Benchmark]
        public Task BenchmarkDelegator() => _delegator.SendAsync(new TestMessage());

        [Benchmark]
        public Task BenchmarkCqrsLite() => _eventPublisher.Publish(new TestCqrsliteEvent());

        [Benchmark]
        public Task BenchmarkMediatr() => _mediator.Publish(new TestMediatrNotif());

        #endregion Benchmarks

        #region Handlers

        private Task handleMessage(TestMessage message, CancellationToken ct) => Task.CompletedTask;
        private Task handleCqrsliteMessage(TestCqrsliteEvent message, CancellationToken ct) => Task.CompletedTask;

        class TestCqrsliteEvent : IEvent
        {
            public Guid Id { get; set; }
            public int Version { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }

        class TestMediatrNotif : INotification { }

        class TestMediatrNotifHandler : INotificationHandler<TestMediatrNotif>
        {
            public Task Handle(TestMediatrNotif notification, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        #endregion Handlers
    }
}