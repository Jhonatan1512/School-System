using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class DocenteService : IDocenteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        private readonly ICursoRepository _cursoRepository;
         
        public DocenteService(
            ApplicationDbContext context, 
            IMatriculaRepository matriculaRepository, 
            IPeriodoAcademicoRepository periodoAcademicoRepository,
            IAsignacionDocenteRepository asignacionDocenteRepository,
            IDocenteRepository docenteRepository,
            ICursoRepository cursoRepository) 
        {
            _context = context;
            _matriculaRepository = matriculaRepository;
            _periodoAcademicoRepository = periodoAcademicoRepository;
            _docenteRepository = docenteRepository;
            _asignacionDocenteRepository = asignacionDocenteRepository;
            _cursoRepository = cursoRepository;
        }

        public async Task<IEnumerable<DocenteDto>> GetAllsync()
        {
            var datos = from docente in _context.Docentes
                        join usuarios in _context.Users
                        on docente.UsuarioId equals usuarios.Id
                        where docente.EsActivo == true
                        select new DocenteDto
                        {
                            Id = docente.Id,
                            Nombres = docente.Nombres,
                            Apellidos = docente.Apellidos,
                            Dni = docente.Dni,
                            Email = usuarios.Email,
                        };
            return await datos.ToListAsync();
        }
        
        public async Task<DocenteDto> GetByDniAsync(string dni)
        {
            var datos = from docente in _context.Docentes
                        join usuarios in _context.Users
                        on docente.UsuarioId equals usuarios.Id
                        where docente.Dni == dni 
                        select new DocenteDto
                        {
                            Id = docente.Id,
                            Nombres = docente.Nombres,
                            Apellidos = docente.Apellidos,
                            Dni = docente.Dni,
                            Email = usuarios.Email,
                        };
            return await datos.FirstOrDefaultAsync();
        }

        public async Task<DetalleCursoDto> ObtenerDetalleCursoAsync(int docenteId, int cursoId, int seccionId, int periodoId)
        {
            bool esSuProfesor = await _asignacionDocenteRepository.ExisteAsignacionAsync(docenteId, cursoId, seccionId, periodoId);
            if (!esSuProfesor) throw new Exception("Acceso Denegado a Esta Sección");

            var curso = await _context.Cursos
                .Include(c => c.Competencias)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null) throw new Exception("Curso no encontrado");

            var matriculas = await _context.Matriculas
                .Include(m => m.Alumno)
                .Include(m => m.Grado)
                .Include(m => m.Seccion)
                .Include(m => m.DetallesMatriculas)
                    .ThenInclude(d => d.Calificaciones)
                .Where(m => m.SeccionId == seccionId && m.PeriodoAcademicoId == periodoId)
                .ToListAsync();

            var trimestre = await _context.Trimestres.FirstOrDefaultAsync(t => t.EstadoActivo);
            string nombreTrimestreActivo = trimestre?.Nombre ?? "Trimestre no definido";

            var alumnosDto = matriculas
                .Where(m => m.DetallesMatriculas.Any(d => d.CursoId == cursoId))
                .Select(m =>
                {
                    var detalle = m.DetallesMatriculas.First(d => d.CursoId == cursoId);

                    return new AlumnoNotaDto
                    {
                        NombreCurso = curso.Nombre,
                        Aula = $"{(m.Grado != null ? m.Grado.Nombre : "")} {(m.Seccion != null ? m.Seccion.Nombre : "")}".Trim(),
                        AlumnoId = m.Alumno!.Id,
                        NombreCompleto = $"{m.Alumno.Nombre} {m.Alumno.Apellidos}",
                        DetalleMatriculaId = detalle.Id,
                        NotasRegistradas = detalle.Calificaciones.Select(c => new NotasRegistradasDto
                        {
                            copetenciaId = c.CompetenciaId,
                            Nota = c.Nota,
                        }).ToList(),
                        Trimestre = nombreTrimestreActivo
                    };
                }).ToList();

            return new DetalleCursoDto
            {
                Competencias = curso.Competencias.Select(c => new CompetenciaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                }).ToList(),
                Alumnos = alumnosDto,
            };
        }

        public async Task<List<DashboardDocenteDto>> ObtenerMiDashboardAsync(string usuarioId)
        {
            var docente = await _docenteRepository.ObtenerPorUsuarioAsync(usuarioId);
            if (docente == null) throw new Exception("Perfil de Docente no Encontrado");

            var periodActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodActivo == null) throw new Exception("No hay Periodo Académico Activo");

            var asignaciones = await _asignacionDocenteRepository.ObtenerAsignacionCompletaDocenteAsync(docente.Id, periodActivo.Id);
            if(!asignaciones.Any()) return new List<DashboardDocenteDto>();

            var seccionesIds = asignaciones.Select(a => a.SeccionId).Distinct().ToList();

            var matriculas = await _matriculaRepository.ObtenerAlumnosPorSeccionPeriodoAsync(seccionesIds, periodActivo.Id);

            var dashboard = asignaciones.Select(asignacion =>
            {
                var alumnosCurso = matriculas.Where(m => m.SeccionId == asignacion.SeccionId &&
                m.DetallesMatriculas.Any(d => d.CursoId == asignacion.CursoId))
                .Select(m =>
                {
                    var detallesCurso = m.DetallesMatriculas.First(d => d.CursoId == asignacion.CursoId);

                    return new AlumnoBasicoDto
                    {
                        AlumnoId = m.Alumno!.Id,
                        NombreCompelto = $"{m.Alumno.Nombre} {m.Alumno.Apellidos}",
                        DetalleMatriculaId = detallesCurso.Id,
                    };
                }).ToList();
                return new DashboardDocenteDto
                {
                    CursoId = asignacion.CursoId,
                    NombreCurso = asignacion.Curso!.Nombre,
                    SeccionId = asignacion.SeccionId,
                    Aula = $"{asignacion.Curso.Grado?.Nombre ?? "Sin Grado"}{asignacion.Seccion?.Nombre ?? "Sin Sección"}",
                    PeriodoTrimestre = periodActivo.Nombre,
                    Competencias = asignacion.Curso.Competencias.Select(c => new CompetenciaDto
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                    }).ToList(),
                    AlumnosMatriculados = alumnosCurso
                };
            }).ToList();
            return dashboard;                  
        }
    }
}
