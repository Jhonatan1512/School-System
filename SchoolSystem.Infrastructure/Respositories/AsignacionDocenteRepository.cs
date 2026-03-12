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

        public async Task<AsignacionDocente> CrearAsignacionAsync(AsignacionDocente dto)
        {
             _context.AsignacionDocentes.Add(dto);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> ExisteAsignacionAsync(int docenteId, int cursoId, int seccionId, int periodoId)
        {
            return await _context.AsignacionDocentes.AnyAsync(a => a.DocenteId == docenteId &&
            a.CursoId == cursoId && a.SeccionId ==  seccionId && a.PeriodoAcademicoId == periodoId);
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

        public async Task<List<AsignacionDocente>> ObtenerPorSeccionPeriodoAsync(int seccionId, int periodoId)
        {
            return await _context.AsignacionDocentes.Include(a => a.Docente)
                .Where(a => a.SeccionId == seccionId && a.PeriodoAcademicoId == periodoId).ToListAsync();
        }
    }
}
