# Pidget ASP.NET Core Middleware

Error reporting middleware for ASP.NET Core.

## Installation

Installing using the dotnet CLI:

```
dotnet add package Pidget.AspNet
```

*More [installation options available at NuGet.](https://www.nuget.org/packages/Pidget.AspNet/)*

## Usage

Exceptions thrown inside the application are automatically captured and sent to Sentry.

### Setup

In Startup.cs add the `Pidget.AspNet.Setup` namespace to your usings.

```csharp
using Pidget.AspNet.Setup;
```

In the `ConfigureServices` method, configure your DSN using the `AddPidgetMiddleware` extension method.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddPidgetMiddleware(o => o.Dsn = "<YOUR_DSN>");
```

In the `Configure` method in Startup.cs, configure your application to use the pidget error reporting middleware.
Make sure you you add the Pidget middleware before your application code, like in the below example (using MVC).

```csharp
public void Configure(IApplicationBuilder app)
{
    // ...

    app.UsePidgetMiddleware();
    app.UseMvc();
```

**See also:** [Startup.cs](https://github.com/mausworks/pidget/blob/master/examples/Pidget.AspNetExample/Startup.cs) in Pidget.AspNetExample

## Configuring callbacks

Callbacks for before and- after send, can be configured with both setup options.

Using configuration:

```csharp
service.AddPidgetMiddleware(config.GetSection("Sentry"), callbacks =>
{
    callbacks.BeforeSendAsync(async (builder, http) => /* ... */);
    callbacks.AfterSendAsync(async (response, http) => /* ... */));
});
```

Or by using a setup-action:

```csharp
service.AddPidgetMiddleware(sentry =>
{
    sentry.Dsn = "YOUR_DSN";
    sentry.Callbacks.BeforeSendAsync(async (builder, http) => /* ... */);
    sentry.Callbacks.AfterSendAsync(async (response, http) => /* ... */));
});
```

**Note:** The after-send callback might return a response of `null`, if rate-limiting has occurred.

## Combining with other error handlers or middleware

The middleware is designed to be unintrusive. Any exceptions caught by the middleware will be re-thrown after being captured.
No special considerations have to be made when combining this middleware with others. However; the order your middleware are configured in may effect which errors are reported, for instance: If the pidget error reporting middleware invokes middleware which suppresses an exception, that exception will not be caught. As such, is it wise to configure your application pipeline to have the pidget exception reporting middleware invoke your application code (see the graph below) and in the [example application](https://github.com/mausworks/pidget/blob/master/examples/Pidget.AspNetExample/Startup.cs#L20-L29).

![request pipeline](https://user-images.githubusercontent.com/8259221/32704132-a0e9dbc4-c800-11e7-86ab-671f804c1a9b.png)


