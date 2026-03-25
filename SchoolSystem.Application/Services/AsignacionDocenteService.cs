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
    public class AsignacionDocenteService : IAsignacionDocenteService
    {
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;
        public AsignacionDocenteService(IAsignacionDocenteRepository asignacionDocenteRepository)
        {
            _asignacionDocenteRepository = asignacionDocenteRepository;  
        }

        public async Task<List<AsignacionDocenteDto>> AsignarCursoAsync(AsignacionDocenteCreateDto dto)
        {
            var resultado = new List<AsignacionDocenteDto>();

            foreach (var cursoId in dto.CursosIds)
            {
                bool existe = await _asignacionDocenteRepository.ExisteAsignacionAsync(
                    dto.DocenteId, cursoId, dto.SeccionId, dto.PeriodoAcademicoId);
                if (existe) throw new Exception($"El docente ya se encuentra asignado al curso {cursoId} en esta sección en el periodo actual");

                var nuevaAsignacion = new AsignacionDocente
                {
                    DocenteId = dto.DocenteId,
                    CursoId = cursoId,
                    GradoId = dto.GradoId,
                    SeccionId = dto.SeccionId,
                    PeriodoAcademicoId = dto.PeriodoAcademicoId,
                };

                var creada = await _asignacionDocenteRepository.CrearAsignacionAsync(nuevaAsignacion);

                resultado.Add(new AsignacionDocenteDto
                {
                    Id = creada.Id,
                    DocenteId = creada.DocenteId,
                    CursoId = creada.CursoId,
                    GradoId = creada.GradoId,
                    SeccionId = creada.SeccionId,
                    PeriodoAcademicoId = creada.PeriodoAcademicoId
                });
            }
            return resultado;
        }

        public async Task<IEnumerable<GetAsignación>> obtenerDocentesAsignadosAsync()
        {
            var asignacion = await _asignacionDocenteRepository.GetAllAsync();

            return asignacion.Select(a => new GetAsignación
            {
                NombreDocente = $"{a.Docente.Nombres} {a.Docente.Apellidos}",
                Dni = a.Docente.Dni,
                NombreCurso = a.Curso.Nombre,
                NombreAula = $"{a.Grado.Nombre}{a.Seccion.Nombre}",
                NombrePeriodo = a.PeriodoAcademico.Nombre,
                Estado = a.Docente.EsActivo.ToString()
            });
        }
    }
}
