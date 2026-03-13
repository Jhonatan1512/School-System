using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class AlumnoService : IAlumnoService
    {
        private readonly ApplicationDbContext _context;
        public readonly IAlumnoRespository _alumnoRepository;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        private readonly IMatriculaRepository _matriculaRepository; 
        private readonly IAsignacionDocenteRepository _signacionDocenteRepository;
        private readonly ITrimestreRepository _trimestreRepository;
        public AlumnoService(
            ApplicationDbContext context,
            IAlumnoRespository alumnoRepository, 
            IPeriodoAcademicoRepository periodoAcademicoRepository, 
            IMatriculaRepository matriculaRepository,  
            IAsignacionDocenteRepository signacionDocenteRepository,
            ITrimestreRepository trimestreRepository)
        {
            _context = context; 
            _alumnoRepository = alumnoRepository;
            _periodoAcademicoRepository = periodoAcademicoRepository;
            _matriculaRepository = matriculaRepository;
            _signacionDocenteRepository = signacionDocenteRepository;
            _trimestreRepository = trimestreRepository;

        }
        public async Task<IEnumerable<AlumnoDto>> GetAll()
        {
            var query = from alumno in _context.Alumnos 
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        }; 
            return await query.ToListAsync(); 
        }

        public async Task<AlumnoDto?> GetByDniAsync(string dni)
        {
            var datos = from alumno in _context.Alumnos
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id
                        where alumno.Dni == dni
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        };
            return await datos.FirstOrDefaultAsync();
        }

        public async Task<AlumnoDto?> GetByIdAsync(int id)
        {
            var datos = from alumno in _context.Alumnos
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id
                        where alumno.Id == id
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        };
            return await datos.FirstOrDefaultAsync();
        }

        public async Task<List<DashboardAlumnoDto>> ObtenerMisCursos(string usuarioId)
        {
            var alumno = await _alumnoRepository.ObtenerPorUsuarioAsync(usuarioId);
            if (alumno == null) throw new Exception("Perfil de Alumno no encontrado");

            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodoActivo == null) throw new Exception("No hay Periodo activo");

            var matricula = await _matriculaRepository.ObtenerPorAlumnoPeriodoAsync(alumno.Id, periodoActivo.Id);

            if (matricula == null || !matricula.DetallesMatriculas.Any())
                return new List<DashboardAlumnoDto>();
            
            var seccionId = matricula.SeccionId;

            var asignaciones = await _signacionDocenteRepository.ObtenerPorSeccionPeriodoAsync(seccionId, periodoActivo.Id);

            var dashboard = matricula.DetallesMatriculas.Select(d =>
            {
                var asignacionDocente = asignaciones.FirstOrDefault(a => a.CursoId == d.CursoId);

                return new DashboardAlumnoDto
                {
                    cursoId = d.CursoId,
                    NombreCurso = d.Curso!.Nombre,
                    NombreDocente = asignacionDocente?.Docente != null
                        ? $"{asignacionDocente.Docente.Nombres} {asignacionDocente.Docente.Apellidos}" : "Si Docente asignado",
                };
            }).ToList();
            return dashboard;
        }

        public async Task<DetalleCursoAlumnoDto> ObtenerDetalleCursoAsync(int lumnoId, int cursoId, int periodoId)
        {
            var matriculas = await _context.Matriculas
                .Include(m => m.DetallesMatriculas)
                .ThenInclude(d => d.Calificaciones)
                .FirstOrDefaultAsync(m => m.AlumnoId == lumnoId && m.PeriodoAcademicoId == periodoId);
            if (matriculas == null) throw new Exception("El alumno no está matriculado en este periodo");

            var detalleCurso = matriculas.DetallesMatriculas.FirstOrDefault(d => d.CursoId == cursoId);
            if (detalleCurso == null) throw new Exception("El alumno no esta matriculado en este curso");

            var curso = await _context.Cursos
                .Include(c => c.Competencias)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            var asignacion = await _context.AsignacionDocentes
                .Include(a => a.Docente)
                .FirstOrDefaultAsync(a => a.CursoId == cursoId && a.SeccionId == matriculas.SeccionId && a.PeriodoAcademicoId == periodoId);

            var trimestre = await _context.Trimestres.Include(t => t.PeriodoAcademico)
                .FirstOrDefaultAsync(t => t.EstadoActivo);

            string nombreTrimestreActivo = trimestre?.Nombre ?? "Trimestre no definido";

            var competencias = curso!.Competencias.Select(comp =>
            {
                var calificaciones = detalleCurso.Calificaciones.FirstOrDefault(c => c.CompetenciaId == comp.Id);

                return new CompetenciasNotaDto
                {
                    CompetenciaId = comp.Id,
                    NombreCompetencia = comp.Nombre,
                    Nota = calificaciones?.Nota ?? "Sin Calificar",
                    NombreTrimestre = nombreTrimestreActivo,
                };
            }).ToList();

            return new DetalleCursoAlumnoDto
            {
                NombreCurso = curso.Nombre,
                NombreDocente = asignacion != null ? $"{asignacion.Docente.Nombres.Trim() } {asignacion.Docente.Apellidos}" : "Sin Docente Asignado",
                Competencias = competencias,
            }; 

        }
    }
}
