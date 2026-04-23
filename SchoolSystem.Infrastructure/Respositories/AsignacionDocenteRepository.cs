using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
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
    public class AsignacionDocenteRepository : IAsignacionDocenteRepository
    {
        private readonly ApplicationDbContext _context;
        public AsignacionDocenteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
         
        public async Task ActualizarAsignacionAsync(int id, AsignacionDocente asignacion)
        {
            _context.AsignacionDocentes.Update(asignacion);
            await _context.SaveChangesAsync(); 
        }

        public async Task<AsignacionDocente> CrearAsignacionAsync(AsignacionDocente dto)
        { 
             _context.AsignacionDocentes.Add(dto); 
            await _context.SaveChangesAsync();
            return dto; 
        }

        public async Task EliminarAsignacionAsync(int id)
        {
            await _context.AsignacionDocentes.Where(a =>  a.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> ExisteAsignacionAsync(int docenteId, int cursoId, int seccionId, int periodoId)
        {
            return await _context.AsignacionDocentes.AnyAsync(a => a.DocenteId == docenteId &&
            a.CursoId == cursoId && a.SeccionId ==  seccionId && a.PeriodoAcademicoId == periodoId);
        }

        public async Task<IEnumerable<AsignacionDocente>> GetAllAsync()
        {
            return await _context.AsignacionDocentes.Include(a => a.Docente)
                            .Include(a => a.Curso)
                            .Include(a => a.Grado)
                            .Include(a => a.Seccion)
                            .Include(a => a.PeriodoAcademico)
                            .Where(a => a.PeriodoAcademico!.EstadoActivo)
                            .ToListAsync();
        }

        public async Task<List<AsignacionDocente>> ObtenerAsignacionCompletaDocenteAsync(int docenteId, int periodoId)
        {
            return await _context.AsignacionDocentes.Include(a => a.Curso)
                .ThenInclude(c => c!.Grado)
                .Include(a => a.Curso)
                .ThenInclude(c => c!.Competencias)
                .Include(a => a.Seccion)
                .Where(a => a.DocenteId == docenteId && a.PeriodoAcademicoId == periodoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AsignacionDocente>> ObtenerPorGradoSeccion(int gradoId, int seccionId)
        {
            return await _context.AsignacionDocentes.Include(a => a.Docente)
                        .Include(a => a.Curso)
                        .Include(a => a.Grado)
                        .Include(a => a.Seccion)
                        .Include(a => a.PeriodoAcademico)
                        .Where(a => a.GradoId == gradoId && a.SeccionId == seccionId && a.PeriodoAcademico!.EstadoActivo)
                        .ToListAsync();
        }

        public async Task<AsignacionDocente?> ObtenerPorIdAsync(int id)
        {
            return await _context.AsignacionDocentes.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AsignacionDocente>> ObtenerPorPeriodoAsync(int periodoId)
        {
            return await _context.AsignacionDocentes.Include(a => a.Curso)
                           .Include(a => a.Docente)
                           .Include(a => a.Seccion)
                           .Where(a => a.PeriodoAcademicoId == periodoId)
                           .ToListAsync();
        }

        public async Task<List<AsignacionDocente>> ObtenerPorSeccionPeriodoAsync(int seccionId, int periodoId)
        {
            return await _context.AsignacionDocentes.Include(a => a.Docente)
                .Where(a => a.SeccionId == seccionId && a.PeriodoAcademicoId == periodoId).ToListAsync();
        }

        public async Task<int> ObtenerHorasCubiertasCursoAsyn(int cursoId, int gradoId, int seccionId, int periodoId, int excluirId)
        {
            return await _context.AsignacionDocentes
                .Where(a => a.CursoId == cursoId &&
                            a.GradoId == gradoId &&
                            a.SeccionId == seccionId &&
                            a.PeriodoAcademicoId == periodoId &&
                            a.Id != excluirId)
                .SumAsync(a => a.HorasAsignadas);

        }

        public async Task<int> ObtenerHorasTotalesDocenteAsync(int docenteId, int periodoId, int excluirId)
        {
            return await _context.AsignacionDocentes
                .Where(a => a.DocenteId == docenteId &&
                            a.PeriodoAcademicoId == periodoId &&
                            a.Id != excluirId)
                .SumAsync(a => a.HorasAsignadas);
        }
    }
}
