using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.UseCases;
using Domain;
using Domain.Commands;
using Domain.DomainEvents;
using Domain.Repositories;
using Infrastructure.DomainEventHandlers;
using ReadSide.Products.Repositories;
using SimpleInjector;
using Xer.Delegator;
using Xer.Delegator.Registrations;
using Xer.Delegator.Resolvers;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            using(CancellationTokenSource cts = new CancellationTokenSource())
            {
                App app = Setup(args);
                await app.StartAsync(args, cts.Token);
            }
        }

        private static App Setup(string[] args)
        {            
            // Simple injector.
            Container container = new Container();
            
            // Write side repository.
            container.RegisterSingleton<IProductRepository>(() =>
                // Use in-memory repository, decorated by a PublishingProductRepository which publishes domain events from the Product.
                new PublishingProductRepository(new InMemoryProductRepository(), container.GetInstance<IMessageDelegator>())
            );

            // Register read side services.
            // In this sample, we will just have a read side repository 
            // which serves as a thin layer between the app layer and the data layer.
            container.RegisterSingleton<IProductReadSideRepository, InMemoryProductReadSideRepository>();

            // Register command handlers and event handlers to the container.
            container.Register<RegisterProductCommandHandler>();
            container.Register<ActivateProductCommandHandler>();
            container.Register<DeactivateProductCommandHandler>();
            container.Register<ProductDomainEventsHandler>();

            // Register console app use cases.
            container.RegisterCollection(typeof(IUseCase), typeof(IUseCase).Assembly);

            // Message handler resolver.
            container.RegisterSingleton<IMessageHandlerResolver>(() => 
            {
                // Register command handlers to the message handler registration. 
                // Commands can only have one handler so use SingleMessageHandlerRegistration.
                SingleMessageHandlerRegistration commandHandlerRegistration = RegisterCommandHandlers(container);

                // Register event handlers to the message handler registration. 
                // Events can have multiple handlers so use MultiMessageHandlerRegistration.
                MultiMessageHandlerRegistration eventHandlerRegistration = RegisterEventHandlers(container);

                // Combine command handlers and event handlers.
                return new CompositeMessageHandlerResolver(new IMessageHandlerResolver[]
                {
                    commandHandlerRegistration.BuildMessageHandlerResolver(),
                    eventHandlerRegistration.BuildMessageHandlerResolver()
                });
            });

            // Message delegator.
            container.RegisterSingleton<IMessageDelegator, MessageDelegator>();

            return new App(container);
        }

        private static SingleMessageHandlerRegistration RegisterCommandHandlers(Container container)
        {
            // Register command handlers to the message handler registration. 
            // Commands can only have one handler so use SingleMessageHandlerRegistration.
            var commandHandlerRegistration = new SingleMessageHandlerRegistration();

            // ActivateProductCommand
            commandHandlerRegistration.Register<RegisterProductCommand>((message, ct) =>
            {
                var handler = container.GetInstance<RegisterProductCommandHandler>();
                return handler.HandleRegisterProductCommandAsync(message, ct);
            });

            // ActivateProductCommand
            commandHandlerRegistration.Register<ActivateProductCommand>((message, ct) =>
            {
                var handler = container.GetInstance<ActivateProductCommandHandler>();
                return handler.HandleActivateProductCommandAsync(message, ct);
            });

            // DeactivateProductCommand
            commandHandlerRegistration.Register<DeactivateProductCommand>((message, ct) =>
            {
                var handler = container.GetInstance<DeactivateProductCommandHandler>();
                return handler.HandleDeactivateProductCommandAsync(message, ct);
            });

            return commandHandlerRegistration;
        }

        private static MultiMessageHandlerRegistration RegisterEventHandlers(Container container)
        {
            // Register event handlers to the message handler registration. 
            // Events can have multiple handlers so use MultiMessageHandlerRegistration.
            var eventHandlerRegistration = new MultiMessageHandlerRegistration();

            // ProductRegisteredEvent
            eventHandlerRegistration.Register<IDomainEvent>((message, ct) =>
            {
                ProductRegisteredEvent domainEvent = message as ProductRegisteredEvent;
                if (domainEvent != null)
                {
                    // Handle only if domain event is a ProductRegisteredEvent.
                    var handler = container.GetInstance<ProductDomainEventsHandler>();
                    return handler.HandleProductRegisteredEventAsync(domainEvent, ct);
                }

                // Do nothing.
                return Task.CompletedTask;
            });

            // ProductActivatedEvent
            eventHandlerRegistration.Register<IDomainEvent>((message, ct) =>
            {
                ProductActivatedEvent domainEvent = message as ProductActivatedEvent;
                if (domainEvent != null)
                {
                    // Handle only if domain event is a ProductActivatedEvent.
                    var handler = container.GetInstance<ProductDomainEventsHandler>();
                    return handler.HandleProductActivatedEventAsync(domainEvent, ct);
                }

                // Do nothing.
                return Task.CompletedTask;
            });

            // ProductDeactivatedEvent
            eventHandlerRegistration.Register<IDomainEvent>((message, ct) =>
            {
                ProductDeactivatedEvent domainEvent = message as ProductDeactivatedEvent;
                if (domainEvent != null)
                {
                    // Handle only if domain event is a ProductDeactivatedEvent.
                    var handler = container.GetInstance<ProductDomainEventsHandler>();
                    return handler.HandleProductDeactivatedEventAsync(domainEvent, ct);
                }

                // Do nothing.
                return Task.CompletedTask;
            });

            return eventHandlerRegistration;
        }
    }
}
