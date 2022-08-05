using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UnioDailyTeams.Infrastructure;
using Topshelf;

namespace UnioDailyTeams
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder().
                UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddSingleton(typeof(ServiceBaseLifetime));
                });

            HostFactory.Run(x =>
            {
                x.Service<ServiceBaseLifetime>(sc =>
                {
                    sc.ConstructUsing(s => host.Build().Services.GetRequiredService<ServiceBaseLifetime>());
                    sc.WhenStarted((s, c) => s.Start(c));
                    sc.WhenStopped((s, c) => s.Stop(c));
                });

                x.RunAsLocalSystem()
                    .DependsOnEventLog()
                    .StartAutomatically()
                    .EnableServiceRecovery(rc => rc.RestartService(1));

                x.SetDescription("Envia aviso de reunião diaria.");
                x.SetDisplayName("Unio Daily Teams");
                x.SetServiceName("Unio Daily Teams");
            });

            await Task.CompletedTask;
        }
    }
}
