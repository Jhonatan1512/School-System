using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class GradoService : IGradoSevice
    {
        private readonly ApplicationDbContext _context;
        public GradoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<GradoDetalleDto?> GetGradoDetalle(int id)
        {
            return await _context.Grados.Where(g => g.Id == id)
                    .Select(g => new GradoDetalleDto
                    {
                        Id = g.Id,
                        Nombre = g.Nombre,
                        Cursos = g.Cursos.Select (c => new CursoSimple {
                            Id = c.Id,
                            Nombre = c.Nombre,
                        }).ToList()
                    }).FirstOrDefaultAsync();
        }
    }
}
