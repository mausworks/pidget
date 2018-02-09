# Pidget ASP.NET Middleware implementation

This project is a very minimal implementation of the ASP.NET Middleware.

In [Startup.cs](https://github.com/mausworks/pidget/blob/master/examples/Pidget.AspNetExample/Startup.cs)
the middleware is configured and added to the request pipeline.

Another middleware is also configured to handle exceptions, the [`OnExceptionMiddleware`](https://github.com/mausworks/pidget/blob/master/examples/Pidget.AspNetExample/OnExceptionMiddleware.cs).
It is effectively an "after-send" middleware.

Example before and after-send callbacks are created in [SentryCallbacks.cs](https://github.com/mausworks/pidget/blob/master/examples/Pidget.AspNetExample/SentryCallbacks.cs).
