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
    public class CalificacionRepository : ICalificacionRepository
    {
        private readonly ApplicationDbContext _context;
        public CalificacionRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Calificacion> ActualizarCalificacionAsync(Calificacion calificacion)
        {
            _context.Calificaciones.Update(calificacion);
            await _context.SaveChangesAsync();
            return calificacion;
        }

        public async Task<Calificacion?> ObtenerCalificacionExistente(int detalleMatriculaId, int competenciaId, int trimestreId)
        {
            return await _context.Calificaciones.FirstOrDefaultAsync(c => c.DetalleMatriculaId == detalleMatriculaId &&
                c.CompetenciaId == competenciaId && c.TrimestreId == trimestreId);
        }

        public async Task<Calificacion> RegistrarCalificacion(Calificacion calificacion)
        {
            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();
            return calificacion;
        }
    }
}
