using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ActivityTest;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((services) =>
            {
                services.AddMassTransit(busConfig =>
                {
                    busConfig.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("amqps://guest:guest@localhost:5672");

                        cfg.ConfigureEndpoints(context);
                    });

                    busConfig.AddSagaStateMachine<MyTestStateMachine, MyTestState>(cfg =>
                    {
                        cfg.UseInMemoryOutbox();
                    }).EntityFrameworkRepository(repo =>
                    {
                        repo.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        repo.AddDbContext<MyStateDbContext, MyStateDbContext>((provider, builder) =>
                        {
                            var connectionString = @"Server=(localdb)\mssqllocaldb; Database=MyTestSagaState; trusted_connection=yes; Persist Security Info=True; MultipleActiveResultSets=True";
                            builder.UseSqlServer(connectionString, m =>
                            {
                                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            });
                        });
                    });
                });

                services.AddSingleton<IHostedService, MassTransitServiceHost>();
            });

        await builder.RunConsoleAsync();
    }
}
