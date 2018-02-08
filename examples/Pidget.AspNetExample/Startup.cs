using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pidget.AspNet.Setup;

namespace Pidget.AspNetExample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
            => services.AddPidgetMiddleware(
                Configuration.GetSection("ExceptionReporting"), callbacks =>
                {
                    callbacks.BeforeSend(SentryCallbacks.BeforeSendAsync);
                    callbacks.AfterSend(SentryCallbacks.AfterSendAsync);
                });


        public void Configure(IApplicationBuilder app)
        {
            // Additional exception handling.
            app.UseMiddleware<OnExceptionMiddleware>();

            app.UsePidgetMiddleware();

            // Throwing application code.
            app.Run(_ => throw new Exception("You shall not pass!"));
        }
    }
}
