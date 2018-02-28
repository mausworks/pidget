# Pidget Client

A simple, easy-to-use Sentry client. Can be used in most .NET applications to capture exceptions or messages.

## Installation

Installing using the dotnet CLI:
```
dotnet add package Pidget.Client
``` 

*More [installation options available at NuGet.](https://www.nuget.org/packages/Pidget.Client/)*

## Usage

Client initialization:

```csharp
var dsn = Dsn.Create("<your DSN>");
var client = Sentry.CreateClient(dsn);
```

Capturing an exception:

```csharp
    // ...
}
catch (Exception ex)
{
    var response = await client.CaptureAsync(e => e.SetException(ex));
    
    // ...
}
```

Capturing a message:

```csharp
var eventId = await client.CaptureAsync(e => e.SetMessage("Foo"));
```

Using the sentry event builder API:

```csharp
var response = await client.CaptureAsync(e => e
    .SetException(exception)
    .SetErrorLevel(ErrorLevel.Fatal)
    .SetTransaction("/index")
    .SetMessage("Whoops!")
    .AddTag("ios_version", "8.0")
    .AddExtraData("payload", payload)
    .AddFingerprintData("environment:dev"));
```
