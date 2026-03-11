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
    public class CursoRepository : ICursoRepository
    { 
        private readonly ApplicationDbContext _context;
        public CursoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarCursoAsync(Curso curso)
        {
            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task AgregarCursoAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosAsync()
        {
            return await _context.Cursos.Include(c => c.Competencias).ToListAsync();
        }

        public async Task<List<Curso>> ObtenerPorGrado(int gradoId)
        {
            return await _context.Cursos.Where(c => c.GradoId == gradoId).ToListAsync();
        }

        public async Task<Curso?> ObtenerPorIdAsync(int id)
        {
            return await _context.Cursos.Include(c => c.Competencias).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
