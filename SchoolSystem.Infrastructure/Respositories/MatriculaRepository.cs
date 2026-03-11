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
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly ApplicationDbContext _context;
        public MatriculaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task ActualizarMatricula(Matricula matricula)
        {
            return _context.SaveChangesAsync();
        }

        public async Task AgregaratriculaAsync(Matricula matricula)
        {
            await _context.Matriculas.AddAsync(matricula);
            await _context.SaveChangesAsync();
        }

        public async Task<Matricula?> ObtenerDetallerId(int id)
        {
            return await _context.Matriculas.Include(m => m.DetallesMatriculas).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Matricula?> ObtenerPorAlumnoPeriodoAsync(int alumnoId, int periodoId)
        {
            return await _context.Matriculas.FirstOrDefaultAsync(m => m.AlumnoId == alumnoId && m.PeriodoAcademicoId == periodoId);
        }

        public async Task<List<Matricula>> ObtenerPorAulaAsync(int gradoId, int seccionId, int periodoId)
        {
            return await _context.Matriculas.Include(m => m.Alumno).Include(m => m.Grado).Include(m => m.Seccion)
                .Where(m => m.GradoId == gradoId && m.SeccionId == seccionId && m.PeriodoAcademicoId == periodoId).ToListAsync();
        }
    }
}
