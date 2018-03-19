# PIDGET 

[![Build status](https://travis-ci.org/mausworks/pidget.svg?branch=master)](https://travis-ci.org/mausworks/pidget)
[![Codecov](https://img.shields.io/codecov/c/github/mausworks/pidget.svg)](https://codecov.io/gh/mausworks/pidget)

Pidget is an exception and error-reporting library for **[Sentry.io](https://sentry.io/)!** 

If you are looking for extensive and configurable exception handling in ASP.NET Core, then have a look at the [ASP.NET Middleware](https://github.com/mausworks/pidget/tree/master/src/Pidget.AspNet). For any other applications you can use [client](https://github.com/mausworks/pidget/tree/master/src/Pidget.Client).

If you have any questions, ideas or requests, please open a [new issue](https://github.com/mausworks/pidget/issues/new)!

## Can I use it?

Pidget targets [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support), which adds support for most (somewhat modern) .NET applications, e.g. .NET Core, Xamarin, Mono, UWP.

* * * * 

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

An easy-to use Sentry client. Basic by design.

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
