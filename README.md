![Pidget logo](https://user-images.githubusercontent.com/8259221/36802949-8fc2ad48-1cb6-11e8-9f7d-c444cf991c8b.png)

Error reporting for C# and ASP.NET Core using [sentry.io](https://sentry.io).

[![Build Status](https://travis-ci.org/mausworks/pidget.svg?branch=master)](https://travis-ci.org/mausworks/pidget)

### Pidget Client [![NuGet](https://img.shields.io/nuget/dt/Pidget.Client.svg)](https://nuget.org/packages/Pidget.Client)

A lightweight client for sending events to Sentry.

[Documentation & source](https://github.com/mausworks/pidget/tree/master/src/Pidget.Client)

* * *

### Pidget ASP.NET Middleware [![NuGet](https://img.shields.io/nuget/dt/Pidget.AspNet.svg)](https://nuget.org/packages/Pidget.AspNet)


ASP.NET Core middleware for automatically capturing application errors.

[Documentation & source](https://github.com/mausworks/pidget/tree/master/src/Pidget.AspNet)

## Target framework

All libraries target [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support).

## [Features](https://docs.sentry.io/clientdev/overview/#writing-an-sdk)

- DSN configuration
- Graceful failures (e.g. Sentry server is unreachable)
- Setting attributes (e.g. tags and extra data)
- Support for Linux, Windows and OS X (where applicable)
- Automated error capturing (e.g. uncaught exception handlers)
- Logging framework integration *(Can be configured with before/after-send hook)*
- Non-blocking event submission
- ~~Basic~~ **configurable** data sanitization (e.g. filtering out values that look like passwords)
- Context data helpers (e.g. setting the current user, recording breadcrumbs)
- Event sampling
- Honor Sentry’s HTTP 429 Retry-After header
- Pre and Post event send hooks
- **Not possible/viable:** Local variable values in stacktrace
