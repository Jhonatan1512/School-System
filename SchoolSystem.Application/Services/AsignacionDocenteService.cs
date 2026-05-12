using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class AsignacionDocenteService : IAsignacionDocenteService
    {
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IPlanEstudioRepository _planEstudioRepository;

        public AsignacionDocenteService(
            IAsignacionDocenteRepository asignacionDocenteRepository,
            IDocenteRepository docenteRepository,
            IPlanEstudioRepository planEstudioRepository)
        {
            _asignacionDocenteRepository = asignacionDocenteRepository;
            _docenteRepository = docenteRepository;
            _planEstudioRepository = planEstudioRepository;
        } 

        public async Task ActualizarAsignacionAsync(int id, AsignacionDocenteDto dto)
        {
            var asignacionExiste = await _asignacionDocenteRepository.ObtenerPorIdAsync(id);
            if (asignacionExiste == null)
            {
                throw new Exception("Registro de asignación no encontrado");
            }

            var docente = await _docenteRepository.ObtenerPorId(dto.DocenteId);
            if (docente == null) throw new Exception("Docente no encontrado");

            var planEstudioInfo = await _planEstudioRepository.ObtenerPorIdAsync(dto.PlanEstudioId);
            if (planEstudioInfo == null) throw new Exception($"Plan de Estudio ID {dto.PlanEstudioId} no encontrado");

            if (planEstudioInfo.PeriodoAcademicoId != dto.PeriodoAcademicoId)
            {
                throw new Exception($"Inconsistencia detectada: El curso '{planEstudioInfo.Curso?.Nombre}' pertenece a un periodo distinto al actual. Operación rechazada.");
            }

            var asignacionesDelPeriodo = await _asignacionDocenteRepository.ObtenerPorPeriodoAsync(dto.PeriodoAcademicoId);

            string jornadaActivaEnPeriodo = asignacionesDelPeriodo
                .Where(a => a.Id != id && a.PlanEstudio != null)
                .Select(a => a.PlanEstudio!.Jornada.ToString().Trim().ToUpper())
                .FirstOrDefault(j => !string.IsNullOrEmpty(j)) ?? "";

            string jornadaNuevaAsignacion = planEstudioInfo.Jornada.ToString().Trim().ToUpper() ?? "JER";

            if (!string.IsNullOrEmpty(jornadaActivaEnPeriodo) && jornadaActivaEnPeriodo != jornadaNuevaAsignacion)
            {
                throw new Exception($"Conflicto de jornada: El periodo actual está configurado como {jornadaActivaEnPeriodo}. No puedes asignar un curso con jornada {jornadaNuevaAsignacion}.");
            }

            int horasAcumuladasDocente = await _asignacionDocenteRepository
                .ObtenerHorasTotalesDocenteAsync(dto.DocenteId, dto.PeriodoAcademicoId, id);

            if (horasAcumuladasDocente + dto.HorasAsignadas > docente.MaxHorasLectivas)
            {
                throw new Exception($"El docente {docente.Nombres} excedería su limite de {docente.MaxHorasLectivas}h. Ya tiene {horasAcumuladasDocente}h asignadas");
            }

            int horasYaCubiertasCurso = await _asignacionDocenteRepository
                .ObtenerHorasCubiertasPlanEstudioAsync(dto.PlanEstudioId, dto.GradoId, dto.SeccionId, dto.PeriodoAcademicoId, id);

            if (horasYaCubiertasCurso + dto.HorasAsignadas > planEstudioInfo.HorasSemanales)
            {
                int disponible = planEstudioInfo.HorasSemanales - horasYaCubiertasCurso;
                throw new Exception($"El curso {planEstudioInfo.Curso!.Nombre} solo tiene {disponible}h no cubiertas en esta sección");
            }

            asignacionExiste.DocenteId = dto.DocenteId;
            asignacionExiste.PlanEstudioId = dto.PlanEstudioId;
            asignacionExiste.GradoId = dto.GradoId;
            asignacionExiste.SeccionId = dto.SeccionId;
            asignacionExiste.HorasAsignadas = dto.HorasAsignadas;
            asignacionExiste.PeriodoAcademicoId = dto.PeriodoAcademicoId;

            await _asignacionDocenteRepository.ActualizarAsignacionAsync(id, asignacionExiste);
        }

        public async Task<List<AsignacionDocenteDto>> AsignarCursoAsync(AsignacionDocenteCreateDto dto)
        {
            var resultado = new List<AsignacionDocenteDto>();

            var docente = await _docenteRepository.ObtenerPorId(dto.DocenteId);
            if (docente == null || !docente.EsActivo)
                throw new Exception("El docente no esta activo");

            int nuevasHorasTotalesDTO = dto.PlanesEstudio.Sum(c => c.HorasAsignadas);

            int cargaActualDb = await _asignacionDocenteRepository.ObtenerHorasTotalesDocenteAsync(dto.DocenteId, dto.PeriodoAcademicoId, 0);

            if (cargaActualDb + nuevasHorasTotalesDTO > docente.MaxHorasLectivas)
            {
                throw new Exception($"El docente {docente.Nombres} tiene {cargaActualDb}h y excedería el límite con esta asignación.");
            }

            var asignacionesDelPeriodo = await _asignacionDocenteRepository.ObtenerPorPeriodoAsync(dto.PeriodoAcademicoId);
            string jornadaActivaEnPeriodo = asignacionesDelPeriodo
                .Where(a => a.PlanEstudio != null)
                .Select(a => a.PlanEstudio!.Jornada.ToString().Trim().ToUpper())
                .FirstOrDefault(j => !string.IsNullOrEmpty(j)) ?? "";

            foreach (var itemPlan in dto.PlanesEstudio)
            {
                var planEstudioInfo = await _planEstudioRepository.ObtenerPorIdAsync(itemPlan.PlanEstudioId);
                if (planEstudioInfo == null) throw new Exception($"Plan de Estudio ID {itemPlan.PlanEstudioId} no encontrado");

                if (planEstudioInfo.PeriodoAcademicoId != dto.PeriodoAcademicoId)
                {
                    throw new Exception($"Inconsistencia detectada: El curso '{planEstudioInfo.Curso?.Nombre}' pertenece a un periodo inactivo/diferente. Operación rechazada.");
                }

                string jornadaPlanActual = planEstudioInfo.Jornada.ToString().Trim().ToUpper() ?? "JER";

                if (!string.IsNullOrEmpty(jornadaActivaEnPeriodo) && jornadaActivaEnPeriodo != jornadaPlanActual)
                {
                    throw new Exception($"Conflicto de jornada: El periodo actual trabaja con jornada {jornadaActivaEnPeriodo}. No se puede asignar el curso '{planEstudioInfo.Curso!.Nombre}' porque tiene jornada {jornadaPlanActual}.");
                }

                if (string.IsNullOrEmpty(jornadaActivaEnPeriodo))
                {
                    jornadaActivaEnPeriodo = jornadaPlanActual;
                }

                int horasYaCubiertasEnSeccion = await _asignacionDocenteRepository
                    .ObtenerHorasCubiertasPlanEstudioAsync(itemPlan.PlanEstudioId, dto.GradoId, dto.SeccionId, dto.PeriodoAcademicoId, 0);

                if (horasYaCubiertasEnSeccion + itemPlan.HorasAsignadas > planEstudioInfo.HorasSemanales)
                {
                    int disponible = planEstudioInfo.HorasSemanales - horasYaCubiertasEnSeccion;
                    throw new Exception($"El curso {planEstudioInfo.Curso!.Nombre} solo tiene {disponible}h disponible en esta sección");
                }

                var nuevaAsignacion = new AsignacionDocente
                {
                    DocenteId = dto.DocenteId,
                    PlanEstudioId = itemPlan.PlanEstudioId,
                    GradoId = dto.GradoId,
                    SeccionId = dto.SeccionId,
                    PeriodoAcademicoId = dto.PeriodoAcademicoId,
                    HorasAsignadas = itemPlan.HorasAsignadas,
                };

                var creada = await _asignacionDocenteRepository.CrearAsignacionAsync(nuevaAsignacion);

                resultado.Add(new AsignacionDocenteDto
                {
                    Id = creada.Id,
                    DocenteId = creada.DocenteId,
                    PlanEstudioId = creada.PlanEstudioId,
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
                NombreCurso = a.PlanEstudio!.Curso!.Nombre,
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

        public async Task<IEnumerable<GetAsignación>> ObtenerPorDniDocenteAsync(string dniDocente)
        {
            var asignaciones = await _asignacionDocenteRepository.ObtenerPorDniDocente(dniDocente);

            return asignaciones.Select(a => new GetAsignación
            {
                Id = a.Id,
                NombreDocente = $"{a.Docente!.Nombres} {a.Docente.Apellidos}",
                Dni = a.Docente.Dni,
                NombreCurso = a.PlanEstudio!.Curso!.Nombre,
                NombreAula = $"{a.Grado!.Nombre}{a.Seccion!.Nombre}",
                HorasAsignadas = a.HorasAsignadas,
                NombrePeriodo = a.PeriodoAcademico!.Nombre,
                Estado = a.Docente.EsActivo.ToString()
            });
        }

        public async Task<IEnumerable<GetAsignación>> ObtenerPorGradoSeccionAsync(int gradoId, int seccionId)
        {
            var asignacion = await _asignacionDocenteRepository.ObtenerPorGradoSeccion(gradoId, seccionId);

            return asignacion.Select(a => new GetAsignación
            {
                Id = a.Id,
                NombreDocente = $"{a.Docente!.Nombres} {a.Docente.Apellidos}",
                Dni = a.Docente.Dni,
                NombreCurso = a.PlanEstudio!.Curso!.Nombre,
                NombreAula = $"{a.Grado!.Nombre}{a.Seccion!.Nombre}",
                HorasAsignadas = a.HorasAsignadas,
                NombrePeriodo = a.PeriodoAcademico!.Nombre,
                Estado = a.Docente.EsActivo.ToString()
            });
        }
    }
}