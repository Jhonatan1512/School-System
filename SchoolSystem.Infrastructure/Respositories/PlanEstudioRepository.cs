using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums; 
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class PlanEstudioRepository : IPlanEstudioRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanEstudioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAsync(PlanEstudio planEstudio)
        {
            _context.PlanEstudios.Update(planEstudio);
            await _context.SaveChangesAsync();
        }
         
        public async Task<PlanEstudio> CrearAsync(PlanEstudio planEstudio)
        {
            _context.PlanEstudios.Add(planEstudio);
            await _context.SaveChangesAsync();
            return planEstudio;
        }

        public async Task EliminarAsync(int id)
        {
            await _context.PlanEstudios.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<PlanEstudio>> GetAllAsync()
        {
            return await _context.PlanEstudios
                                 .Include(p => p.Curso) 
                                 .ToListAsync();
        }

        public async Task<PlanEstudio?> ObtenerPorIdAsync(int id)
        {
            return await _context.PlanEstudios
                                 .Include(p => p.Curso) 
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PlanEstudio>> ObtenerPorJornadaAsync(TipoJornada jornada)
        {
            return await _context.PlanEstudios
                                 .Include(p => p.Curso)
                                 .Where(p => p.Jornada == jornada)
                                 .ToListAsync();
        }
        
        public async Task<bool> ExistePlanAsync(int cursoId, TipoJornada tipoJornada)
        {
            return await _context.PlanEstudios.AnyAsync(p => p.CursoId == cursoId && p.Jornada == tipoJornada);
        }
    }
}