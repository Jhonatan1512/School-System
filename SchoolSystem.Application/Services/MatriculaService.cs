using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        
        private readonly IMatriculaRepository _matriculaRepository; 
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IUsuarioRepository _ussuarioRepository;
        public MatriculaService( 
            IMatriculaRepository matriculaRepository, 
            IPeriodoAcademicoRepository periodoAcademico, 
            ICursoRepository cursoRepository,
            IUsuarioRepository usuarioRepository
            )
        {
            _matriculaRepository = matriculaRepository;
            _periodoAcademicoRepository = periodoAcademico;
            _cursoRepository = cursoRepository;
            _ussuarioRepository = usuarioRepository;
        }

        public async Task<bool> ActualizarMatricula(int matriculaId, ActualizarMatriculaDto dto)
        {
            var matriculaExiste = await _matriculaRepository.ObtenerDetallerId(matriculaId);
            if (matriculaExiste == null)
                throw new Exception("Matricula no encontrada");

            if(matriculaExiste.GradoId != dto.GradoId)
            {
                var nuevosCursos = await _cursoRepository.ObtenerPorGrado(dto.GradoId);
                if (!nuevosCursos.Any())
                    throw new Exception("No se puede cambiar a este grado porque no tiene cursos registrados");

                matriculaExiste.GradoId = dto.GradoId;

                matriculaExiste.DetallesMatriculas.Clear();

                foreach(var curso in nuevosCursos)
                {
                    matriculaExiste.DetallesMatriculas.Add(new DetalleMatricula
                    {
                        CursoId = curso.Id,
                    });
                }
            }
            matriculaExiste.SeccionId = dto.SeccionId;

            await _matriculaRepository.ActualizarMatricula(matriculaExiste);
            return true;
        }

        public async Task<bool> AgregarMatriculaDetallerAsync(MatriculaDto dto)
        {
            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodoActivo == null)
                throw new Exception("No se puede matricular: No hay periodo académico activo");

            var matriculaExiste = await _matriculaRepository.ObtenerPorAlumnoPeriodoAsync(dto.AlumnoId, periodoActivo.Id);
            if (matriculaExiste != null)
                throw new Exception("El alumno ya esta matriculado en el periodo académico actual");

            var cursosGrado = await _cursoRepository.ObtenerPorGrado(dto.GradoId);
            if (!cursosGrado.Any())
                throw new Exception("No se puede matricular: el grado seleccionado no tiene cursos registrados");

            var nuevaMatricula = new Matricula
            {
                AlumnoId = dto.AlumnoId,
                GradoId = dto.GradoId,
                SeccionId = dto.SeccionId,
                PeriodoAcademicoId = periodoActivo.Id,

                DetallesMatriculas = cursosGrado.Select(curso => new DetalleMatricula
                {
                    CursoId = curso.Id,
                }).ToList()
            };

            await _matriculaRepository.AgregaratriculaAsync(nuevaMatricula);
            return true;
        }

        public async Task<List<GetMatriculadosSeccionDto>> ObtenerMatriculadosPorAula(int gradoId, int seccionId)
        {
            var periodosActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodosActivo == null)
                throw new Exception("No hay un periodo académico activo");

            var matriculas = await _matriculaRepository.ObtenerPorAulaAsync(gradoId, seccionId, periodosActivo.Id);

            var listaAlumnos = new List<GetMatriculadosSeccionDto>();

            foreach(var m in matriculas)
            {
                string correo = await _ussuarioRepository.GetEmailById(m.Alumno!.UsuarioId);

                listaAlumnos.Add(new GetMatriculadosSeccionDto
                {
                    MatriculaId = m.Id,
                    AlumnoId = m.Id,
                    Nombre = m.Alumno!.Nombre,
                    Apellidos = m.Alumno.Apellidos,
                    Dni = m.Alumno.Dni,
                    Correo = correo,
                    Aula = $"{m.Grado!.Nombre}{m.Seccion!.Nombre}",
                    Estado = m.Alumno.Estado.ToString(),
                });
            }
            return listaAlumnos;
        }
    }
}
