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
                Configuration.GetSection("ExceptionReporting"));

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<OnExceptionMiddleware>();
            app.UsePidgetMiddleware();

            app.Run(_ => throw new Exception("You shall not pass!"));
        }
    }
}
