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
        public AlumnoService(
            ApplicationDbContext context,
            IAlumnoRespository alumnoRepository, 
            IPeriodoAcademicoRepository periodoAcademicoRepository, 
            IMatriculaRepository matriculaRepository, 
            IAsignacionDocenteRepository signacionDocenteRepository)
        {
            _context = context; 
            _alumnoRepository = alumnoRepository;
            _periodoAcademicoRepository = periodoAcademicoRepository;
            _matriculaRepository = matriculaRepository;
            _signacionDocenteRepository = signacionDocenteRepository;
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
                        ? $"{asignacionDocente.Docente.Nombres}{asignacionDocente.Docente.Apellidos}" : "Si Docente asignado",

                    Competencias = d.Curso.Competencias.Select(c => new CompetenciasDto
                    {
                        Id = c.Id,
                        Nombre = c.Nombre
                    }).ToList()
                };
            }).ToList();
            return dashboard;
        }
    }
}
