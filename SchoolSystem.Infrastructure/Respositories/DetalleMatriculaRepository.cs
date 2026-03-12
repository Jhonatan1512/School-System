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
    public class DetalleMatriculaRepository : IDetalleMatriculaRepository
    {
        private readonly ApplicationDbContext _context;
        public DetalleMatriculaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<DetalleMatricula> ObtenerDetallePorId(int id)
        {
            return await _context.DetalleMatriculas.Include(d => d.Matricula).FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
