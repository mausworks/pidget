# Pidget ASP.NET Middleware

Error reporting middleware for ASP.NET Core.

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

```csharp
public void Configure(IApplicationBuilder app)
{
    // ...

    app.UsePidgetMiddleware();
```

**See also:** [Startup.cs](https://github.com/mausworks/pidget/blob/feature/aspnet/examples/Pidget.AspNetExample/Startup.cs) in Pidget.AspNetExample

## Combining with other error handlers / middleware

The middleware is designed to be unintrusive. Any exceptions caught by the middleware will be re-thrown after being captured.

![Request-pipeline](https://user-images.githubusercontent.com/8259221/32703808-8999ba84-c7fb-11e7-9959-03d61ff6cbe3.png)

As such, no special considerations have to be made when combining this middleware with others. However; the order your middleware are configured in may effect which errors are reported, for instance: If the pidget error reporting middleware invokes middleware which suppresses an exception, that exception will not be caught. As such, is it wise to configure your application pipeline to have the pidget exception reporting middleware invoke your application code (as in the above graph).
