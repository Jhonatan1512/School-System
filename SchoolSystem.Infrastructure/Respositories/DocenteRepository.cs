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
    public class DocenteRepository : IDocenteRepository
    {
        private readonly ApplicationDbContext _context;

        public DocenteRepository(ApplicationDbContext context)
        {
            _context = context;
        } 

        public async Task ActualizarDoncenteAsync(Docente docente) 
        {
            _context.Docentes.Update(docente); 
            await _context.SaveChangesAsync();  
        } 

        public async Task<Docente> CrearDocenteAsync(Docente docente)
        {
            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();
            return docente;
        }

        public async Task<Docente?> ObtenerPorDniAsync(string dni)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Dni == dni);
        }

        public async Task<Docente?> ObtenerPorId(int id)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<Docente?> ObtenerPorUsuarioAsync(string usuarioId)
        {
            return await _context.Docentes.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId && u.EsActivo);
        }

        public async Task<Docente?> ObtenerActivoAsync(int id)
        {
            return await _context.Docentes.Where(d => d.Id == id && d.EsActivo).FirstOrDefaultAsync();
        }

        public async Task<Docente?> PerfilDocenteAsync(string usuarioId)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
        }

        public async Task<AsignacionDocente?> ObtenerAsignacionTutoriaAsync(int docenteId, int periodoId)
        {
            return await _context.AsignacionDocentes
                .Include(a => a.Docente)
                .Include(a => a.PlanEstudio)
                    .ThenInclude(p => p!.Curso)
                .Include(a => a.Grado)
                .Include(a => a.Seccion)
                .Where(a => a.DocenteId == docenteId && a.PeriodoAcademicoId == periodoId && a.PlanEstudio!.Curso!.Nombre.ToUpper().Contains("Tutoría")).FirstOrDefaultAsync();
                
        }

        public async Task<List<Matricula>> ObtenerAlumnosSeccionAsync(int gradoId, int seccionId, int periodoId)
        {
            return await _context.Matriculas
                .Include(m => m.Alumno)
                .Include(m => m.Seccion)
                .Include(m => m.Grado)
                .Include(m => m.PeriodoAcademico)
                .Where(m => m.GradoId == gradoId && m.SeccionId == seccionId && m.PeriodoAcademicoId == periodoId)
                .OrderBy(m => m.Alumno!.Apellidos)
                .ToListAsync();
        }

        public async Task<bool> ValidarTutoriaAlumnoAsync(int docenteId, int alumnoId, int periodoId)
        {
            return await _context.AsignacionDocentes
                .AnyAsync(a => a.DocenteId == docenteId &&
                            a.PeriodoAcademicoId == periodoId &&
                            a.PlanEstudio!.Curso!.Nombre.Contains("Tutoría") &&
                            _context.Matriculas.Any(m => m.AlumnoId == alumnoId &&
                                                        m.GradoId == a.GradoId &&
                                                        m.SeccionId == a.SeccionId &&
                                                        m.PeriodoAcademicoId == periodoId));
        }

        public async Task<List<Calificacion>> ObtenerNotasCompletasAsync(int alumnoId, int periodoId)
        {
            return await _context.Calificaciones
                .Include(c => c.Trimestre)
                .Include(n => n.Competencia)
                    .ThenInclude(c => c!.Curso)
                .Include(n => n.DetalleMatricula)
                .Where(n => n.DetalleMatricula!.Matricula!.AlumnoId == alumnoId && n.DetalleMatricula.Matricula.PeriodoAcademicoId == periodoId)
                .ToListAsync();
        }
    }
}
