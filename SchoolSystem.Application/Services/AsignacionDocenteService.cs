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
        private readonly IDocenteRepository _docenteRepository;
        private readonly ICursoRepository _cursoRepository;

        public AsignacionDocenteService(IAsignacionDocenteRepository asignacionDocenteRepository, IDocenteRepository docenteRepository, ICursoRepository cursoRepository)
        {
            _asignacionDocenteRepository = asignacionDocenteRepository;
            _docenteRepository = docenteRepository;
            _cursoRepository = cursoRepository;
        }
          
        public async Task ActualizarAsignacionAsync(int id, AsignacionDocenteDto dto) 
        {
            var asiganacionExiste = await _asignacionDocenteRepository.ObtenerPorIdAsync(id);
            if(asiganacionExiste == null)
            {
                throw new Exception("Registro de asignación no encontrado"); 
            }

            var docente = await _docenteRepository.ObtenerPorId(dto.DocenteId);
            if (docente == null) throw new Exception("Docente no encontrado");

            var cursoInfo = await _cursoRepository.ObtenerPorIdAsync(dto.CursoId);
            if (cursoInfo == null) throw new Exception($"Curso ID {dto.CursoId} no encontrado");

            int horasAcumaladasDocente = await _asignacionDocenteRepository
                .ObtenerHorasTotalesDocenteAsync(dto.DocenteId, dto.PeriodoAcademicoId, id);

            if(horasAcumaladasDocente + dto.HorasAsignadas > docente.MaxHorasLectivas)
            {
                throw new Exception($"El docente {docente.Nombres} exedería su limite de {docente.MaxHorasLectivas}h. Ya tiene {horasAcumaladasDocente}h asignadas");
            }



            var horasMaximas = await _docenteRepository.ObtenerPorId(dto.DocenteId);
            if (dto.HorasAsignadas > horasMaximas!.MaxHorasLectivas)
            {
                throw new Exception("El docente ya completo sus horas máximas a dictar");
            }

            int horasYaCubiertasCurso = await _asignacionDocenteRepository
                .ObtenerHorasCubiertasCursoAsyn(dto.CursoId, dto.GradoId, dto.SeccionId, dto.PeriodoAcademicoId, id);

            if(horasYaCubiertasCurso + dto.HorasAsignadas > cursoInfo.HorasSemanales)
            {
                int disponible = cursoInfo.HorasSemanales - horasYaCubiertasCurso;
                throw new Exception($"El curso {cursoInfo.Nombre} solo tiene {disponible}h no cubiertas en esta sección");
            }

            asiganacionExiste.DocenteId = dto.DocenteId;
            asiganacionExiste.CursoId = dto.CursoId;
            asiganacionExiste.GradoId = dto.GradoId;
            asiganacionExiste.SeccionId = dto.SeccionId;
            asiganacionExiste.HorasAsignadas = dto.HorasAsignadas;
            asiganacionExiste.PeriodoAcademicoId = dto.PeriodoAcademicoId;

            await _asignacionDocenteRepository.ActualizarAsignacionAsync(id, asiganacionExiste);

        }

        public async Task<List<AsignacionDocenteDto>> AsignarCursoAsync(AsignacionDocenteCreateDto dto)
        { 
            var resultado = new List<AsignacionDocenteDto>();

            var docente = await _docenteRepository.ObtenerPorId(dto.DocenteId);
            if (docente == null || !docente.EsActivo)
                throw new Exception("El docente no esta activo");

            int nuevasHorasTotalesDTO = dto.CursosIds.Sum(c => c.HorasAsignadas);

            int cargaActualDb = await _asignacionDocenteRepository.ObtenerHorasTotalesDocenteAsync(dto.DocenteId, dto.PeriodoAcademicoId, 0);

            if (cargaActualDb + nuevasHorasTotalesDTO > docente.MaxHorasLectivas)
            {
                throw new Exception($"El docente {docente.Nombres} tiene {cargaActualDb}h");
            }

            foreach (var cursoId in dto.CursosIds)
            {
                var cursoInfo = await _cursoRepository.ObtenerPorIdAsync(cursoId.CursoId);
                if (cursoInfo == null) throw new Exception($"Curso ID {cursoId.CursoId} no encontrado");

                int horasYaCubiertasEnSeccion = await _asignacionDocenteRepository
                    .ObtenerHorasCubiertasCursoAsyn(cursoId.CursoId, dto.GradoId, dto.SeccionId, dto.PeriodoAcademicoId, 0);

                if(horasYaCubiertasEnSeccion + cursoId.HorasAsignadas > cursoInfo.HorasSemanales)
                {
                    int disponible = cursoInfo.HorasSemanales - horasYaCubiertasEnSeccion;
                    throw new Exception($"El curso {cursoInfo.Nombre} solo tiene {disponible}h disponible en esta sección");
                }

                var nuevaAsignacion = new AsignacionDocente
                {
                    DocenteId = dto.DocenteId,
                    CursoId = cursoId.CursoId,
                    GradoId = dto.GradoId,
                    SeccionId = dto.SeccionId,
                    PeriodoAcademicoId = dto.PeriodoAcademicoId,
                    HorasAsignadas = cursoId.HorasAsignadas,
                };

                var creada = await _asignacionDocenteRepository.CrearAsignacionAsync(nuevaAsignacion);

                resultado.Add(new AsignacionDocenteDto
                {
                    Id = creada.Id,
                    DocenteId = creada.DocenteId,
                    CursoId = creada.CursoId,
                    GradoId = creada.GradoId,
                    SeccionId = creada.SeccionId,
                    PeriodoAcademicoId = creada.PeriodoAcademicoId,
                    HorasAsignadas = creada.HorasAsignadas
                }); 
            }
            return resultado;
        }
         
        public async Task<PageResponseDto<GetAsignación>> obtenerDocentesAsignadosAsync(int pagina, int cantidad)
        {
            var asignacion = await _asignacionDocenteRepository.GetAllAsync();

            var totalRegistros = asignacion.Count();

            var items = asignacion.OrderBy(a => a.GradoId)
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToList();

            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)cantidad);

            var asignacionesDto = items.Select(a => new GetAsignación
            {
                Id = a.Id,
                NombreDocente = $"{a.Docente!.Nombres} {a.Docente.Apellidos}",
                Dni = a.Docente.Dni,
                NombreCurso = a.Curso!.Nombre,
                NombreAula = $"{a.Grado!.Nombre}{a.Seccion!.Nombre}",
                HorasAsignadas = a.HorasAsignadas,
                NombrePeriodo = a.PeriodoAcademico!.Nombre,
                Estado = a.Docente.EsActivo.ToString()
            }).ToList();

            return new PageResponseDto<GetAsignación>
            {
                Items = asignacionesDto,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
            };
        }

        public async Task<IEnumerable<GetAsignación>> ObtenerPorGradoSeccionAsync(int gradoId, int seccionId)
        {
            var asignacion = await _asignacionDocenteRepository.ObtenerPorGradoSeccion(gradoId, seccionId);

            return asignacion.Select(a => new GetAsignación
            {
                Id = a.Id,
                NombreDocente = $"{a.Docente!.Nombres} {a.Docente.Apellidos}",
                Dni = a.Docente.Dni,
                NombreCurso = a.Curso!.Nombre,
                NombreAula = $"{a.Grado!.Nombre}{a.Seccion!.Nombre}",
                NombrePeriodo = a.PeriodoAcademico!.Nombre,
                Estado = a.Docente.EsActivo.ToString()
            });
        }
    }
}
