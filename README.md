![pidget-txt-sm](https://user-images.githubusercontent.com/8259221/32839163-cbc898c0-ca13-11e7-8624-b8e1dffa31eb.png)

A tiny client for [Sentry](https://sentry.io/).

[![Build Status](https://travis-ci.org/mausworks/pidget.svg?branch=master)](https://travis-ci.org/mausworks/pidget)

## [Supported platforms:](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)

- .NET Core 2.0
- .NET Framework 4.6.1+ (With .NET Core 2.0 SDK)
- Mono 4.6.1
- Xamarin.iOS 5.4
- Xamarin.Android 3.8
- UWP 10.0.16299

## Dependencies

- Newtonsoft.Json `>=9.0.1`

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
    .AddTag("request_method", "POST")
    .AddExtraData("request_body", request.Body)
    .AddFingerprintData("POST", "/index"));
```

## [Features](https://docs.sentry.io/clientdev/overview/#writing-an-sdk)

- DSN configuration (Pidget.Client)
- `planned` ~~Graceful failures (e.g. Sentry server is unreachable)~~
- Setting attributes (e.g. tags and extra data) (Pidget.Client)
- Support for Linux, Windows and OS X (where applicable) (Pidget.Client, Pidget.AspNet)
- Automated error capturing (e.g. uncaught exception handlers) (Pidget.AspNet)
- ~~Logging framework integration~~
- Non-blocking event submission
- Basic data sanitization (e.g. filtering out values that look like passwords) (Pidget.AspNet)
- ~~Context data helpers (e.g. setting the current user, recording breadcrumbs)~~
- Event sampling (Pidget.Client, Pidget.AspNet)
- `planned` ~~Honor Sentry’s HTTP 429 Retry-After header~~
- ~~Pre and Post event send hooks~~
- ~~Local variable values in stacktrace (on platforms where this is possible)~~
