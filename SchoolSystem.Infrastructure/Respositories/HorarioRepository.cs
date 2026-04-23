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
    public class HorarioRepository : IHorarioRepository
    {
        private readonly ApplicationDbContext _context;

        public HorarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExisteCruce(int docenteId, string dia, int horaLectiva, int periodoId)
        {
            return await _context.Horario
                .AnyAsync(h => h.DiaSemana == dia &&
                               h.HoraLectivaId == horaLectiva && 
                               h.AsignacionDocente!.DocenteId == docenteId &&
                               h.AsignacionDocente.PeriodoAcademicoId ==  periodoId);
        }

        public async Task<bool> ExisteCruceSeccion(int seccionId, string dia, int horaLectivaId, int periodoId)
        {
            return await _context.Horario
                .AnyAsync(h => h.DiaSemana == dia && 
                               h.HoraLectivaId == horaLectivaId &&
                               h.AsignacionDocente!.SeccionId == seccionId &&
                               h.AsignacionDocente.PeriodoAcademicoId == periodoId);
        }

        public async Task InsertarRangoAsync(IEnumerable<Horario> horarios)
        {
            await _context.Horario.AddRangeAsync(horarios);
        }

        public async Task LimpiarHorariosPeriodoAsync(int periodoId)
        {
            var horariosExiste = _context.Horario.Where(h => h.AsignacionDocente!.PeriodoAcademicoId == periodoId);

            _context.Horario.RemoveRange(horariosExiste);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Horario>> ObtenerPorGradoSeccionPeriodo(int gradoId, int seccionId, int periodoId)
        {
            return await _context.Horario
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Curso)
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Docente)
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Grado)
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Seccion)
                .Where(h => h.AsignacionDocente!.GradoId == gradoId &&
                            h.AsignacionDocente.SeccionId == seccionId &&
                            h.AsignacionDocente.PeriodoAcademicoId == periodoId)
                .ToListAsync();
        }
    }
}
