# Pidget Sentry Client

A lightweight sentry client for .NET

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
    var eventId = await client.CaptureAsync(e => e.SetException(ex));

    // ...
}
```

Capturing a message:

```csharp
var eventId = await client.CaptureAsync(e => e.SetMessage("Foo"));
```

Using the sentry event builder API:

```csharp
var eventId = await client.CaptureAsync(e => e
    .SetException(exception)
    .SetErrorLevel(ErrorLevel.Fatal)
    .SetTransaction("/index")
    .SetMessage("Whoops!")
    .AddTag("ios_version", "8.0")
    .AddExtraData("payload", payload)
    .AddFingerprintData("environment:dev"));
```

## Supported frameworks

This library targets .NET Standard 2.0 an supports the following platforms.

- .NET Core 2.0
- .NET Framework 4.6.1+ (With .NET Core 2.0 SDK)
- Mono 4.6.1
- Xamarin.iOS 5.4
- Xamarin.Android 3.8
- UWP 10.0.16299

## Dependencies

- Newtonsoft.Json `>=10.0.3`
