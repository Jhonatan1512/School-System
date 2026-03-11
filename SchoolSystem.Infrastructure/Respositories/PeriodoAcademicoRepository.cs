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
    public class PeriodoAcademicoRepository : IPeriodoAcademicoRepository
    {
        private readonly ApplicationDbContext _context;
        public PeriodoAcademicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarPeriodoAsync(PeriodoAcademico periodo)
        {
            _context.PeriodoAcademicos.Update(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task AgregarPeriodoAsync(PeriodoAcademico periodo)
        {
            await _context.PeriodoAcademicos.AddAsync(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task<PeriodoAcademico?> ObtenerPeriodoAcademicoActivo()
        {
            return await _context.PeriodoAcademicos.FirstOrDefaultAsync(p => p.EstadoActivo);
        }

        public async Task<IEnumerable<PeriodoAcademico>> ObtenerTodosasync()
        {
            return await _context.PeriodoAcademicos.ToListAsync();
        }
    }
}
