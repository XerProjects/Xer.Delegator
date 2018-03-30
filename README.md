
# Xer.Delegator
A lightweight in-process, delegate-based message handling library.

# Build
| Branch | Status |
|--------|--------|
| Master | [![Build status](https://ci.appveyor.com/api/projects/status/9gk9go3i21y492ap?svg=true)](https://ci.appveyor.com/project/XerProjects25246/xer-delegator) |
| Dev | [![Build status](https://ci.appveyor.com/api/projects/status/9gk9go3i21y492ap/branch/dev?svg=true)](https://ci.appveyor.com/project/XerProjects25246/xer-delegator/branch/dev) |

# Nuget
[![NuGet](https://img.shields.io/nuget/vpre/xer.delegator.svg)](https://www.nuget.org/packages/Xer.Delegator/)

# Table of contents
* [What is it?](#what-is-it)
* [Installation](#installation)
* [Getting Started](#getting-started)
  * [Message Handler Registration](#message-handler-registration)
    * [Single Message Handler](#single-message-handler)
    * [Multiple Message Handlers](#multiple-message-handlers)
  * [Delegating Messages](#delegating-messages)

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

### Message Handler Registration
#### Single Message Handler

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
        // Commands can only have one handler so use SingleMessageHandlerRegistration.
        var commandHandlerRegistration = new SingleMessageHandlerRegistration();
        
        // Register command handlers to the single message handler registration. 

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

        return commandHandlerRegistration.BuildMessageHandlerResolver();
    });
    
    // Register message delegator to the container. 
    // This can be simply be: services.AddSingleton<IMessageDelegator, MessageDelegator>(), 
    // but I wanted to clearly show that MessageDelegator depends on IMessageHandlerResolver.
    services.AddSingleton<IMessageDelegator, MessageDelegator>((serviceProvider) =>
        // Get the registered instance of IMessageHandlerResolver shown above.
        new MessageDelegator(serviceProvider.GetRequiredService<IMessageHandlerResolver>())
    );
    ...
}
```

#### Multiple Message Handlers

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
        // Events can have multiple handlers so use MultiMessageHandlerRegistration.
        var eventHandlerRegistration = new MultiMessageHandlerRegistration();
        
        // Register event handlers to the message handler registration. 
        
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

        return eventHandlerRegistration.BuildMessageHandlerResolver();
    });
    
    // Register message delegator to the container. 
    // This can be simply be: services.AddSingleton<IMessageDelegator, MessageDelegator>(), 
    // but I wanted to clearly show that MessageDelegator depends on IMessageHandlerResolver.
    services.AddSingleton<IMessageDelegator, MessageDelegator>((serviceProvider) =>
        // Get the registered instance of IMessageHandlerResolver shown above.
        new MessageDelegator(serviceProvider.GetRequiredService<IMessageHandlerResolver>())
    );
    ...
}
```

#### Delegating Messages
All messages can be delegated to one or more message handlers through the MessageDelegator's SendAsync API.

```csharp
// ProductController.cs

private readonly IMessageDelegator _messageDelegator;

public ProductController(IMessageDelegator messageDelegator)
{
    _messageDelegator = messageDelegator;
}

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
