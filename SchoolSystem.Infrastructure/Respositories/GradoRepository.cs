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
    public class GradoRepository : IGradoRepository
    {
        private readonly ApplicationDbContext _context;
        public GradoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Grado> CrearGrado(Grado grado)
        {
            _context.Grados.Add(grado);
            await _context.SaveChangesAsync();
            return grado;
        }

        public async Task<Grado?> ObtenerPorId(int id)
        {
            return await _context.Grados.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
