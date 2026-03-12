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
    public class TrimestreRepository : ITrimestreRepository
    {
        private readonly ApplicationDbContext _context;
        public TrimestreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarTrimestreAsync(Trimestre trimestre)
        {
            _context.Trimestres.Update(trimestre);
            await _context.SaveChangesAsync();
        }

        public async Task<Trimestre> CrearTrimestreAsync(Trimestre trimestre)
        {
            await _context.Trimestres.AddAsync(trimestre);
            await _context.SaveChangesAsync();
            return trimestre;
        }

        public async Task<Trimestre?> ObtenerPorIdAsync(int id)
        {
            return await _context.Trimestres.FindAsync(id);
        }

        public async Task<IEnumerable<Trimestre>> ObtenerPorPeriodo(int periodoAcademicoId)
        {
            return await _context.Trimestres.Where(t => t.PeriodoAcademicoId ==  periodoAcademicoId).ToListAsync();
        }

        public async Task<Trimestre?> ObtenerTrimestreActioPorPeriodo(int periodoAcademicoId)
        {
            return await _context.Trimestres.FirstOrDefaultAsync(t => t.PeriodoAcademicoId==periodoAcademicoId && t.EstadoActivo);
        }
    }
}
