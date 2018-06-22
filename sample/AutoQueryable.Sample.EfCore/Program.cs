using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoQueryable.Sample.EfCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices(services =>
                {
                    services.AddAutofac();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging
                        .AddConsole();
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
