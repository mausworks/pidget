# PIDGET 

Error reporting for C# and ASP.NET Core using [sentry.io](https://sentry.io).

![Pidget logo](https://user-images.githubusercontent.com/8259221/36802949-8fc2ad48-1cb6-11e8-9f7d-c444cf991c8b.png)
[![Build status](https://travis-ci.org/mausworks/pidget.svg?branch=master)](https://travis-ci.org/mausworks/pidget)

## Platform support

All libraries target [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support) and supports most (somewhat modern) .NET applications, e.g. .NET Core, Xamarin, UWP.

## Pidget ASP.NET Middleware 

Captures ASP.NET Core application errors and automatically attaches user & request data.

### [Features](https://docs.sentry.io/clientdev/overview/#writing-an-sdk)

- Support for Linux, Windows and OS X
- Automated error capturing (e.g. uncaught exception handlers)
- ~~Logging framework integration~~ (Can be configured with before/after-send hooks)
- Non-blocking event submission
- ~~Basic~~ **configurable** data sanitization (e.g. filtering out values that look like passwords)
- Context data helpers (e.g. setting the current user, recording breadcrumbs)
- Honor Sentry’s HTTP 429 Retry-After header
- Pre and Post event send hooks

**[Documentation & source](https://github.com/mausworks/pidget/tree/master/src/Pidget.AspNet)**

[![NuGet](https://img.shields.io/nuget/dt/Pidget.AspNet.svg)](https://nuget.org/packages/Pidget.AspNet)

* * * *

## Pidget Client

An easy-to use Sentry client. Basic by design; can be used in most .NET applications, Xamarin, etc &hellip;

### [Features](https://docs.sentry.io/clientdev/overview/#writing-an-sdk)

- DSN configuration
- Graceful failures (e.g. Sentry server is unreachable)
- Setting attributes (e.g. tags and extra data)
- Support for Linux, Windows and OS X
- Non-blocking event submission
- Context data helpers (e.g. setting the current user, recording breadcrumbs)
- Event sampling
- **Not possible/viable:** Local variable values in stacktrace

**[Documentation & source](https://github.com/mausworks/pidget/tree/master/src/Pidget.Client)**

[![NuGet](https://img.shields.io/nuget/dt/Pidget.Client.svg)](https://nuget.org/packages/Pidget.Client)
