using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolSystem.Infrastructure.Data;
using Microsoft.Extensions.Hosting;

namespace SchoolSystem.Api.BackgroundService 
{
    public class CierreTrimestreService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public CierreTrimestreService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContex = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var trimestreExpirados = await dbContex.Trimestres.Where(t => t.EstadoActivo == true && t.FechaCierre < DateTime.Now)
                            .ToListAsync();

                        if( trimestreExpirados.Any() )
                        {
                            foreach (var trimestre in trimestreExpirados )
                            {
                                trimestre.EstadoActivo = false;
                            }

                            await dbContex.SaveChangesAsync(stoppingToken);
                        }

                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error en el vigilante de trimestres: ${ex.Message}");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
