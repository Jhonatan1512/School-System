using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class AlumnoService : IAlumnoService
    { 
        private readonly ApplicationDbContext _context;
        public readonly IAlumnoRespository _alumnoRepository;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        private readonly IMatriculaRepository _matriculaRepository; 
        private readonly IAsignacionDocenteRepository _signacionDocenteRepository;
         
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

        }
        public async Task<PageResponseDto<AlumnoDto>> GetAll(int pagina, int cantidad)
        {
            var query = from alumno in _context.Alumnos 
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id 

                        join matricula in _context.Matriculas 
                        on alumno.Id equals matricula.AlumnoId into matriculasGrupo
                        from m in matriculasGrupo.DefaultIfEmpty()

                        join grado in _context.Grados 
                        on m.GradoId equals grado.Id  into gradosGrupo
                        from g in gradosGrupo.DefaultIfEmpty()

                        join seccion in _context.Secciones
                        on m.SeccionId equals seccion.Id into seccionesGrupo 
                        from s in seccionesGrupo.DefaultIfEmpty()

                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            Aula = (g != null && s != null)
                                    ? $"{g.Nombre}{s.Nombre}" : "No Matriculado",
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!,
                            UsuarioId = usuario.Id,
                        }; 

            var totalRegistro = await query.CountAsync();

            var items = await query
                .OrderBy(a =>a.Apellidos)
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();
             
            var totalPaginas = (int)Math.Ceiling(totalRegistro / (double)cantidad);

            return new PageResponseDto<AlumnoDto>
            {
                Items = items,
                TotalRegistros = totalRegistro,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
            }; 
        }

        public async Task<AlumnoDto?> GetByDniAsync(string dni)
        {
            var datos = from alumno in _context.Alumnos
                        join usuario in _context.Users on alumno.UsuarioId equals usuario.Id

                        join m in _context.Matriculas on alumno.Id equals m.AlumnoId into matriculaGroup
                        from matricula in matriculaGroup.DefaultIfEmpty()

                        join g in _context.Grados on matricula.GradoId equals g.Id into gradoGroup
                        from grado in gradoGroup.DefaultIfEmpty()

                        join s in _context.Secciones on matricula.SeccionId equals s.Id into seccionGroup
                        from secciones in seccionGroup.DefaultIfEmpty()

                        join pa in _context.PeriodoAcademicos on matricula.PeriodoAcademicoId equals pa.Id into periodoGroup
                        from periodo in periodoGroup.DefaultIfEmpty()

                        where alumno.Dni == dni
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            Aula = (grado != null && secciones != null)
                                    ? $"{grado.Nombre}{secciones.Nombre}" : "Sin Aula",

                            gradoId = grado != null ? grado.Id : 0,

                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!,
                            PeriodoAcademico = (periodo != null)
                                        ? $"{periodo.Nombre}" : "Sin Matrícula",

                            MatriculaId = matricula != null ? matricula.Id : 0,
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
                var asignacionDocente = asignaciones.Where(a => a.PlanEstudio!.CursoId == d.CursoId).ToList();

                return new DashboardAlumnoDto
                {
                    cursoId = d.CursoId,
                    NombreCurso = d.Curso!.Nombre,
                    Docentes = asignacionDocente.Select(a => new DocentesCusroDto
                    {
                        Nombre = $"{a.Docente!.Nombres} {a.Docente!.Apellidos}",
                    }).ToList(),
                    NombreAula = $"{matricula.Grado!.Nombre}{matricula.Seccion!.Nombre}"
                };
            }).ToList();
            return dashboard;
        }

        public async Task<DetalleCursoAlumnoDto> ObtenerDetalleCursoAsync(int alumnoId, int cursoId, int periodoId) 
        {
            var matriculas = await _context.Matriculas
                .Include(m => m.DetallesMatriculas)
                .ThenInclude(d => d.Calificaciones)
                .FirstOrDefaultAsync(m => m.AlumnoId == alumnoId && m.PeriodoAcademicoId == periodoId);
            if (matriculas == null) throw new Exception("El alumno no está matriculado en este periodo");

            var detalleCurso = matriculas.DetallesMatriculas.FirstOrDefault(d => d.CursoId == cursoId);
            if (detalleCurso == null) throw new Exception("El alumno no esta matriculado en este curso");

            var curso = await _context.Cursos
                .Include(c => c.Competencias)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            var asignacion = await _context.AsignacionDocentes
                .Include(a => a.Docente)
                .FirstOrDefaultAsync(a => a.PlanEstudio!.CursoId == cursoId && a.SeccionId == matriculas.SeccionId && a.PeriodoAcademicoId == periodoId);

            var trimestre = await _context.Trimestres.Where(t => t.PeriodoAcademicoId == periodoId).OrderBy(t => t.Id)
                .ToListAsync();

            var listacompetenciasNotas = new List<CompetenciasNotaDto>();

            foreach (var comp in curso!.Competencias)
            {
                foreach(var tri in trimestre)
                {
                    var calificaciones = detalleCurso.Calificaciones
                        .FirstOrDefault(c => c.CompetenciaId == comp.Id && c.TrimestreId == tri.Id);

                    listacompetenciasNotas.Add(new CompetenciasNotaDto
                    {
                        CompetenciaId = comp.Id,
                        NombreCompetencia = comp.Nombre,
                        TrimestreId = tri.Id,
                        NombreTrimestre = tri.Nombre,
                        Nota = calificaciones?.Nota ?? "Sin calificar"
                    });
                }
            }

            return new DetalleCursoAlumnoDto
            {
                NombreCurso = curso.Nombre,
                NombreDocente = asignacion != null ? $"{asignacion.Docente!.Nombres.Trim() } {asignacion.Docente.Apellidos}" : "Sin Docente Asignado",
                Competencias = listacompetenciasNotas,
            }; 

        }

        public async Task<IEnumerable<AlumnoDto>> AlumnosSeccionAsync(int gradoId, int seccionId, int periodoId)
        {
            var query = from alumno in _context.Alumnos
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id

                        join matricula in _context.Matriculas
                        on alumno.Id equals matricula.AlumnoId 

                        join grado in _context.Grados
                        on matricula.GradoId equals grado.Id 

                        join seccion in _context.Secciones
                        on matricula.SeccionId equals seccion.Id 

                        where grado.Id == gradoId && seccion.Id == seccionId && alumno.Estado == AlumnoEnum.Activo

                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            Aula = (grado != null && seccion != null)
                                    ? $"{grado.Nombre}{seccion.Nombre}" : "Sin Asignar",
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        };
            return await query.ToListAsync();
        }

        public async Task ActualizarEstadoAsync(int id, ActualizarEstadoDto dto)
        {
            var alumnoExiste = await _alumnoRepository.GetById(id);
            if( alumnoExiste is null )
            {
                throw new KeyNotFoundException("el alumno no existe");
            }

            if(Enum.TryParse<AlumnoEnum>(dto.Estado, true, out var nuevoEstado))
            {
                alumnoExiste.Estado = nuevoEstado;
            } else
            {
                throw new Exception("El estado enviado no es válido");
            } 

            await _alumnoRepository.ActualizarAlumnoAsync(alumnoExiste);            
        }
    }
}
