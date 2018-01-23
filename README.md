
# Xer.Delegator
[![NuGet](https://img.shields.io/nuget/vpre/xer.delegator.svg)](https://www.nuget.org/packages/Xer.Delegator/)

A lightweight in-process message handling library without the boilerplates!

# Table of contents
* [What is it?](#what-is-it)
* [Installation](#installation)
* [Getting Started](#getting-started)
  * [ASP.NET Core](#aspnet-core)
    * [Startup Configuration](#startup-configuration)
    * [Sending Messages](#sending-messages)

## What is it?
This library is developed with a goal to help developers speed up the development of applications by minimizing boilerplates in the code. No message marker interfaces, no message handler interfaces, no pipelines, etc - just define a message class, hook up delegates, and you're good to go!

This makes the library suitable for building prototypes/proof of concept applications, but at the same time, also serve as a lightweight base for your own messaging infrastructure.

## Installation
You can simply clone this repository, build the source, reference the output dll, and code away!

The library has also been published as a Nuget package:
* https://www.nuget.org/packages/Xer.Delegator/

To install Nuget package:
1. Open command prompt
2. Go to project directory
3. Add the package to the project:
    ```csharp
    dotnet add package Xer.Delegator
    ```
4. Restore the packages:
    ```csharp
    dotnet restore
    ```

## Getting Started
The samples follows the CQRS pattern so you will see commands, events, etc.

### ASP.NET Core
#### Startup Configuration

```csharp
// Startup.cs

// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    ...
    // Register message handler resolver to the container. 
    // This is resolved by the MessageDelegator.
    services.AddSingleton<IMessageHandlerResolver>((serviceProvider) =>
    {
        SingleMessageHandlerRegistration commandHandlerRegistration = RegisterCommandHandlers(serviceProvider);
        MultiMessageHandlerRegistration eventHandlerRegistration = RegisterEventHandlers(serviceProvider);

        // Combine command handlers and event handlers into a single message handler resolver.
        return new CompositeMessageHandlerResolver(new IMessageHandlerResolver[]
        {
            commandHandlerRegistration.BuildMessageHandlerResolver(),
            eventHandlerRegistration.BuildMessageHandlerResolver()
        });
    });
    
    // Register message delegator to the container. 
    services.AddSingleton<IMessageDelegator, MessageDelegator>();
    ...
}

// Register all command handlers
private static SingleMessageHandlerRegistration RegisterCommandHandlers(IServiceProvider serviceProvider)
{
    // Register command handlers to the message handler registration. 
    // Commands can only have one handler so use SingleMessageHandlerRegistration.
    var commandHandlerRegistration = new SingleMessageHandlerRegistration();

    // RegisterProductCommand
    commandHandlerRegistration.Register<RegisterProductCommand>((message, ct) =>
    {
        var handler = serviceProvider.GetRequiredService<RegisterProductCommandHandler>();
        return handler.HandleRegisterProductCommandAsync(message, ct);
    });

    // ActivateProductCommand
    commandHandlerRegistration.Register<ActivateProductCommand>((message, ct) =>
    {
        var handler = serviceProvider.GetRequiredService<ActivateProductCommandHandler>();
        return handler.HandleActivateProductCommandAsync(message, ct);
    });

    // DeactivateProductCommand
    commandHandlerRegistration.Register<DeactivateProductCommand>((message, ct) =>
    {
        var handler = serviceProvider.GetRequiredService<DeactivateProductCommandHandler>();
        return handler.HandleDeactivateProductCommandAsync(message, ct);
    });

    return commandHandlerRegistration;
}

// Register event handlers
private static MultiMessageHandlerRegistration RegisterEventHandlers(IServiceProvider serviceProvider)
{
    // Register event handlers to the message handler registration. 
    // Events can have multiple handlers so use MultiMessageHandlerRegistration.
    var eventHandlerRegistration = new MultiMessageHandlerRegistration();
    
    // In the sample, all events are published as IDomainEvent by the PublishingRepository,
    // so register event handlers for IDomainEvent and check if domain event can be handled.

    // ProductRegisteredEvent
    eventHandlerRegistration.Register<IDomainEvent>((message, ct) =>
    {
        ProductRegisteredEvent domainEvent = message as ProductRegisteredEvent;
        if (domainEvent != null)
        {
            // Handle only if domain event is a ProductRegisteredEvent.
            var handler = serviceProvider.GetRequiredService<ProductDomainEventsHandler>();
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
            var handler = serviceProvider.GetRequiredService<ProductDomainEventsHandler>();
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
            var handler = serviceProvider.GetRequiredService<ProductDomainEventsHandler>();
            return handler.HandleProductDeactivatedEventAsync(domainEvent, ct);
        }

        // Do nothing.
        return Task.CompletedTask;
    });

    return eventHandlerRegistration;
}
```
#### Sending Messages
All messages can be sent to one or more message handlers through the MessageDelegator's SendAsync API.

```csharp
// ProductController.cs

// Inject in controller contructor.
private readonly IMessageDelegator _messageDelegator;

[HttpPost]
public async Task<IActionResult> RegisterProduct([FromBody]RegisterProductCommandDto model)
{
    // Convert DTO to domain command.
    RegisterProductCommand command = model.ToDomainCommand();
    // Send command message to handler.
    await _messageDelegator.SendAsync(command);
    return Ok();
}
```
