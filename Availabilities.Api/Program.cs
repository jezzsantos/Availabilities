using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ServiceStack;

namespace Availabilities
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseModularStartup<Startup, StartupActivator>(); });
        }
    }

    public class StartupActivator : ModularStartupActivator
    {
        public StartupActivator(IConfiguration configuration) : base(configuration)
        {
        }
    }
}