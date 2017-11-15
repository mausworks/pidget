![pidget-txt-sm](https://user-images.githubusercontent.com/8259221/32839163-cbc898c0-ca13-11e7-8624-b8e1dffa31eb.png)

[Sentry](https://sentry.io) error reporting for C# and ASP.NET Core.

[![Build Status](https://travis-ci.org/mausworks/pidget.svg?branch=master)](https://travis-ci.org/mausworks/pidget)

## Projects

This repository contains multiple projects. 

### Pidget.Client

A lightweight client for sending events to Sentry.

- [Pidget.Client on NuGet](https://www.nuget.org/packages/Pidget.Client) 
- [Source & documentation](https://github.com/mausworks/pidget/tree/master/src/Pidget.Client)
- [Tests](https://github.com/mausworks/pidget/tree/master/test/Pidget.Client.Test)

### Pidget.AspNet

ASP.NET Core middleware for automatically capturing application errors.

- [Pidget.AspNet on NuGet](https://www.nuget.org/packages/Pidget.AspNet) 
- [Source & documentation](https://github.com/mausworks/pidget/tree/master/src/Pidget.AspNet)
- [Tests](https://github.com/mausworks/pidget/tree/master/test/Pidget.AspNet.Test)

## Target framework 

All libraries target [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support).

## [Features](https://docs.sentry.io/clientdev/overview/#writing-an-sdk)

- DSN configuration (Pidget.Client)
- `planned` ~~Graceful failures (e.g. Sentry server is unreachable)~~
- Setting attributes (e.g. tags and extra data) (Pidget.Client)
- Support for Linux, Windows and OS X (where applicable) (Pidget.Client, Pidget.AspNet)
- Automated error capturing (e.g. uncaught exception handlers) (Pidget.AspNet)
- ~~Logging framework integration~~
- Non-blocking event submission (Pidget.Client, Pidget.AspNet)
- Basic data sanitization (e.g. filtering out values that look like passwords) (Pidget.AspNet)
- ~~Context data helpers (e.g. setting the current user, recording breadcrumbs)~~
- Event sampling (Pidget.Client, Pidget.AspNet)
- `planned` ~~Honor Sentry’s HTTP 429 Retry-After header~~
- ~~Pre and Post event send hooks~~
- ~~Local variable values in stacktrace (on platforms where this is possible)~~
