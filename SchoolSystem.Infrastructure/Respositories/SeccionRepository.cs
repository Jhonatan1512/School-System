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
    public class SeccionRepository : ISeccionRepository
    {
        private readonly ApplicationDbContext _context;
        public SeccionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAsync(Seccion seccion)
        {
            _context.Secciones.Update(seccion);
            await _context.SaveChangesAsync();
        }

        public async Task AgregarSeccionAsync(Seccion seccion)
        {
            _context.Secciones.Add(seccion);
            await _context.SaveChangesAsync();
        }

        public async Task<Seccion?> ObtenerPorIdAsync(int id)
        {
            return await _context.Secciones.FindAsync(id);
        }

        public async Task<IEnumerable<Seccion>> ObtenerTodosAsync()
        {
            return await _context.Secciones.ToListAsync();
        }
    }
}
