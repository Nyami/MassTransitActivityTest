using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ActivityTest;

public class MassTransitServiceHost : IHostedService, IDisposable
{
    private readonly IBusControl _busControl;
    private readonly ILogger<MassTransitServiceHost> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer = null!;

    public MassTransitServiceHost(IBusControl busControl, ILogger<MassTransitServiceHost> logger, IServiceProvider serviceProvider)
    {
        _busControl = busControl;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MassTransit Service Host");

        using (var scope = _serviceProvider.CreateScope())
        {
            var dataContext = scope.ServiceProvider.GetRequiredService<MyStateDbContext>();
            await dataContext.Database.EnsureCreatedAsync();
        }

        await _busControl.StartAsync(cancellationToken);

        var probeResult = _busControl.GetProbeResult(cancellationToken);
        var jsonResult = JsonSerializer.Serialize(probeResult, new JsonSerializerOptions() { WriteIndented = true });
        _logger.LogInformation($"Result of probe:{Environment.NewLine}{jsonResult}");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        _logger.LogInformation("MassTransit Service Host Started");
    }

    private void DoWork(object? state)
    {
        _busControl.Publish<SomeEvent>(new { MyId = Guid.NewGuid() });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping MassTransit Service Host");
        await _busControl.StopAsync(cancellationToken);
        _timer.Change(Timeout.Infinite, 0);
        _logger.LogInformation("MassTransit Service Host Stopped");
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
