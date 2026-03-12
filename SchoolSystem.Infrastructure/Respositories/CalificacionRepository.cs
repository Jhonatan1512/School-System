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
    public class CalificacionRepository : ICalificacionRepository
    {
        private readonly ApplicationDbContext _context;
        public CalificacionRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<Calificacion> RegistrarCalificacion(Calificacion calificacion)
        {
            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();
            return calificacion;
        }
    }
}
