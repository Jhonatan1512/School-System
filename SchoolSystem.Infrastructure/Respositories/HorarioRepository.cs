using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Repositories
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
                               h.AsignacionDocente.PeriodoAcademicoId == periodoId);
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
            if (horarios != null && horarios.Any())
            {
                var horariosParaGuardar = horarios.Select(h => new Horario
                {
                    AsignacionDocenteId = h.AsignacionDocenteId,
                    HoraLectivaId = h.HoraLectivaId,
                    DiaSemana = h.DiaSemana
                }).ToList();

                await _context.Horario.AddRangeAsync(horariosParaGuardar);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LimpiarHorariosPeriodoAsync(int periodoId)
        {
            var horariosExiste = await _context.Horario
                .Where(h => h.AsignacionDocente!.PeriodoAcademicoId == periodoId)
                .ToListAsync();

            if (horariosExiste.Any())
            {
                _context.Horario.RemoveRange(horariosExiste);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Horario>> ObtenerPorGradoSeccionPeriodo(int gradoId, int seccionId, int periodoId)
        {
            return await _context.Horario
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.PlanEstudio!.Curso)
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Docente)
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Grado)
                .Include(h => h.AsignacionDocente)
                    .ThenInclude(a => a!.Seccion)
                .Where(h => h.AsignacionDocente!.GradoId == gradoId &&
                            h.AsignacionDocente.SeccionId == seccionId &&
                            h.AsignacionDocente.PeriodoAcademicoId == periodoId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}