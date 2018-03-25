using System;
using System.IO;
using Domain.Commands;
using Domain.DomainEvents;
using Domain.Repositories;
using Infrastructure.DomainEventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReadSide.Products.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Xer.Delegator;
using Xer.Delegator.Registration;
using Xer.Delegator.Resolvers;

namespace AspNetCoreApp
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        private static readonly string AspNetCoreAppXmlDocPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                                                                              $"{typeof(Startup).Assembly.GetName().Name}.xml");
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Swagger.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "AspNetCore Sample", Version = "v1" });
                c.IncludeXmlComments(AspNetCoreAppXmlDocPath);
            });

            // Write side repository.
            services.AddSingleton<IProductRepository>((serviceProvider) =>
                // Use in-memory repository, decorated by a PublishingProductRepository which publishes domain events from the Product aggregate root.
                new PublishingProductRepository(new InMemoryProductRepository(), serviceProvider.GetRequiredService<IMessageDelegator>())
            );

            // Register read side services.
            // In this sample, we will just have a read side repository 
            // which serves as a thin layer between the app layer and the data layer.
            services.AddSingleton<IProductReadSideRepository, InMemoryProductReadSideRepository>();

            // Register command handlers and event handlers to the container.
            services.AddTransient<RegisterProductCommandHandler>();
            services.AddTransient<ActivateProductCommandHandler>();
            services.AddTransient<DeactivateProductCommandHandler>();
            services.AddTransient<ProductDomainEventsHandler>();

            // Register message delegator to the container.
            services.AddSingleton<IMessageDelegator>((serviceProvider) =>
            {
                // Register command handlers to the message handler registration. 
                // Commands can only have one handler so use SingleMessageHandlerRegistration.
                SingleMessageHandlerRegistration commandHandlerRegistration = RegisterCommandHandlers(serviceProvider);

                // Register event handlers to the message handler registration. 
                // Events can have multiple handlers so use MultiMessageHandlerRegistration.
                MultiMessageHandlerRegistration eventHandlerRegistration = RegisterEventHandlers(serviceProvider);

                // Combine command handlers and event handlers.
                var resolver = CompositeMessageHandlerResolver.Compose(
                    commandHandlerRegistration.BuildMessageHandlerResolver(),
                    eventHandlerRegistration.BuildMessageHandlerResolver()
                );

                return new MessageDelegator(resolver);
            });
        }

        private static SingleMessageHandlerRegistration RegisterCommandHandlers(IServiceProvider serviceProvider)
        {
            // Register command handlers to the message handler registration. 
            // Commands can only have one handler so use SingleMessageHandlerRegistration.
            var commandHandlerRegistration = new SingleMessageHandlerRegistration();

            // ActivateProductCommand
            commandHandlerRegistration.Register<RegisterProductCommand>((message, cancellationToken) =>
            {
                // You can also manually instantiate if that's how you roll.
                var handler = serviceProvider.GetRequiredService<RegisterProductCommandHandler>();
                return handler.HandleRegisterProductCommandAsync(message, cancellationToken);
            });

            // ActivateProductCommand
            commandHandlerRegistration.Register<ActivateProductCommand>((message, cancellationToken) =>
            {
                // You can also manually instantiate if that's how you roll.
                var handler = serviceProvider.GetRequiredService<ActivateProductCommandHandler>();
                return handler.HandleActivateProductCommandAsync(message, cancellationToken);
            });

            // DeactivateProductCommand
            commandHandlerRegistration.Register<DeactivateProductCommand>((message, cancellationToken) =>
            {
                // You can also manually instantiate if that's how you roll.
                var handler = serviceProvider.GetRequiredService<DeactivateProductCommandHandler>();
                return handler.HandleDeactivateProductCommandAsync(message, cancellationToken);
            });

            return commandHandlerRegistration;
        }

        private static MultiMessageHandlerRegistration RegisterEventHandlers(IServiceProvider serviceProvider)
        {
            // Register event handlers to the message handler registration. 
            // Events can have multiple handlers so use MultiMessageHandlerRegistration.
            var eventHandlerRegistration = new MultiMessageHandlerRegistration();

            // ProductRegisteredEvent
            eventHandlerRegistration.Register<ProductRegisteredEvent>((message, cancellationToken) =>
            {
                // You can also manually instantiate if that's how you roll.
                var handler = serviceProvider.GetRequiredService<ProductDomainEventsHandler>();
                return handler.HandleProductRegisteredEventAsync(message, cancellationToken);
            });

            // ProductActivatedEvent
            eventHandlerRegistration.Register<ProductActivatedEvent>((message, cancellationToken) =>
            {
                // You can also manually instantiate if that's how you roll.
                var handler = serviceProvider.GetRequiredService<ProductDomainEventsHandler>();
                return handler.HandleProductActivatedEventAsync(message, cancellationToken);
            });

            // ProductDeactivatedEvent
            eventHandlerRegistration.Register<ProductDeactivatedEvent>((message, cancellationToken) =>
            {
                // You can also manually instantiate if that's how you roll.
                var handler = serviceProvider.GetRequiredService<ProductDomainEventsHandler>();
                return handler.HandleProductDeactivatedEventAsync(message, cancellationToken);
            });

            return eventHandlerRegistration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Hosting environment.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCore Sample V1");
            });

            app.UseMvc();
        }
    }
}
