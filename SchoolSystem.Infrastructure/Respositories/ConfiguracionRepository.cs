using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class ConfiguracionRepository : IConfiguracionRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IPeriodoAcademicoRepository periodoAcademicoRepository;

        public ConfiguracionRepository(ApplicationDbContext context, IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            this.context = context;
            this.periodoAcademicoRepository = periodoAcademicoRepository;
        }
        public async Task<bool> CrearConfiguracionAsync(ConfiguracionGradoSeccion gradoSeccion)
        {
            context.ConfiguracionGradoSecciones.Add(gradoSeccion);            
            return await context.SaveChangesAsync() > 0 ;
        }

        public async Task<ConfiguracionGradoSeccion?> ObtenerConfiguracionEspecificaAsync(int periodoId, int gradoId, int seccionId)
        {
            return await context.ConfiguracionGradoSecciones
                .Include(c => c.Grado)
                .Include(c => c.Seccion)
                .Include(c => c.PeriodoAcademico)
                .FirstOrDefaultAsync(c => c.GradoId == gradoId &&
                                    c.SeccionId == seccionId &&
                                    c.PeriodoacademicoId == periodoId);
        }

        public async Task<List<ConfiguracionGradoSeccion>> GetDetailsByPeriodo(int periodoId)
        {
            return await context.ConfiguracionGradoSecciones.Include(c => c.Grado)
                .Include(c => c.Seccion)
                .Include(C => C.PeriodoAcademico)
                .Where(c => c.PeriodoacademicoId == periodoId)
                .ToListAsync();
        }

        public async Task EliminarConfiguracion(int id)
        {
            await context.ConfiguracionGradoSecciones.Where(c => c.Id == id).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task<ConfiguracionGradoSeccion?> GetconfiguracionById(int id)
        {
            return await context.ConfiguracionGradoSecciones.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task ActualizarConfiguracionAsync(ConfiguracionGradoSeccion data)
        {
            context.ConfiguracionGradoSecciones.Update(data);
            await context.SaveChangesAsync();
        }
    }
}
