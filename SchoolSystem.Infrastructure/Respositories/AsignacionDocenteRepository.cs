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
    public class AsignacionDocenteRepository : IAsignacionDocenteRepository
    {
        private readonly ApplicationDbContext _context;
        public AsignacionDocenteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AsignacionDocente>> ObtenerPorSessionPeriodoAsync(int seccionId, int periodoId)
        {
            return await _context.AsignacionDocentes.Include(a => a.Docente)
                .Where(a => a.SeccionId == seccionId && a.PeriodoAcademicoId == periodoId).ToListAsync();
        }
    }
}
