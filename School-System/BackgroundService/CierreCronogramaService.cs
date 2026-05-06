using SchoolSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 

namespace SchoolSystem.BackgroundService
{
    public class CierreCronogramaService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CierreCronogramaService> _logger; 

        public CierreCronogramaService(IServiceScopeFactory scopeFactory, ILogger<CierreCronogramaService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CierreCronogramaService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var ahora = DateTime.Now; 

                    // Activar cronogramas vigentes
                    var activados = await dbContext.CronogramaMatriculas
                        .Where(c =>
                            !c.EstadoActivo &&
                            c.FechaHoraInicio <= ahora &&
                            c.FechaHoraFin > ahora
                        )
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(c => c.EstadoActivo, true),
                            stoppingToken);

                    // Desactivar cronogramas expirados
                    var desactivados = await dbContext.CronogramaMatriculas
                        .Where(c =>
                            c.EstadoActivo &&
                            c.FechaHoraFin < ahora 
                        )
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(c => c.EstadoActivo, false),
                            stoppingToken);

                    if (activados > 0 || desactivados > 0)
                    {
                        _logger.LogInformation($"Cronogramas actualizados: {activados} activados, {desactivados} desactivados.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error crítico actualizando estados de cronogramas.");
                }

                // Espera 1 minuto antes de volver a revisar
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}