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
    public class CronogramaRepository : ICronogramaRepository
    {
        private readonly ApplicationDbContext _context;

        public CronogramaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarCronogramaAsync(CronogramaMatricula cronograma)
        {
            _context.CronogramaMatriculas.Update(cronograma);
            await _context.SaveChangesAsync();
        }

        public async Task CrearCronogramaAsync(CronogramaMatricula cronograma)
        {
            await _context.CronogramaMatriculas.AddAsync(cronograma);
            await _context.SaveChangesAsync();  
        }

        public async Task<CronogramaMatricula?> ObtenerActivoPorGradoAsync(int gradoId, int periodoId)
        {
            return await _context.CronogramaMatriculas
                         .Where(c => c.GradoId == gradoId && c.PeriodoAcademicoId == periodoId)
                         .FirstOrDefaultAsync();
        }

        public async Task<CronogramaMatricula?> ObtenerPorIdAsync(int id)
        {
            return await _context.CronogramaMatriculas.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CronogramaMatricula>> ObtenerTodosAsync()
        {
            return await _context.CronogramaMatriculas.ToListAsync();
        }
    }
}
