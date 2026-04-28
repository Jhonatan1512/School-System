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
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IGradoRepository _gradoRepository;
        private readonly IPlanEstudioRepository _planEstudioRepository;
        private readonly IPeriodoAcademicoRepository _periodo;

        public CursoService(
            ICursoRepository cursoRepository,
            IGradoRepository gradoRepository,
            IPlanEstudioRepository planEstudioRepository,
            IPeriodoAcademicoRepository periodo) 
        {
            _cursoRepository = cursoRepository;
            _gradoRepository = gradoRepository; 
            _planEstudioRepository = planEstudioRepository;
            _periodo = periodo;
        }

        public async Task<bool> ActualizarCursoCompetenciaAsync(int id, CursoCompetenciaDto dto)
        {
            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(id);
            if (cursoExiste == null) return false;

            cursoExiste.Nombre = dto.Nombre;
            cursoExiste.GradoId = dto.GradoId;

            await _cursoRepository.ActualizarCursoAsync(cursoExiste);
            return true;
        }

        public async Task<Curso> CrearCursoCompetenciaAsync(CrearCursoComptenciasDto dto)
        {
            var periodoActivo = await _periodo.ObtenerPeriodoAcademicoActivo();
            if (periodoActivo == null)
            {
                throw new Exception("El periodo no esta activo");
            }
            if (dto.PlanEstudios.Any())
            {
                var jornadasDistintas = dto.PlanEstudios.Select(p => p.Jornada).Distinct().Count();
                if(jornadasDistintas > 1)
                {
                    throw new Exception("Un curso no puede estar en 2 planes diferentes en el mismo año escolar");
                }
            }

            var jornadaSolicitada = dto.PlanEstudios.First().Jornada;

            bool jornadaDistinta = await _cursoRepository.ExistePlanPoPerido(jornadaSolicitada);
            if (jornadaDistinta)
            {
                throw new Exception("La jornada que esta intentando registrar es distinta a los demás grados");
            }

            bool cursoExiste = await _cursoRepository.ExisteCursoPorNombreYGrado(dto.Nombre, dto.GradoId);
            if (cursoExiste)
            {
                throw new Exception($"El curso '{dto.Nombre}' ya existe en este grado. Si deseas modificar su jornada, edita el curso existente");
            }

            var gradoExiste = await _gradoRepository.ObtenerPorId(dto.GradoId);
            if (gradoExiste == null) throw new Exception("El grado especificado no existe");

            var nuevoCurso = new Curso
            {
                Nombre = dto.Nombre,
                GradoId = dto.GradoId,
                Prioridad = (PrioridadCurso)dto.Prioridad,
                Competencias = dto.Competencias.Select(c => new Competencia
                {
                    Nombre = c.Nombre
                }).ToList(),
                PlanEstudios = dto.PlanEstudios.Select(p => new PlanEstudio
                {
                    Jornada = p.Jornada,
                    HorasSemanales = p.HorasSemanales,
                    HorasMaximasPorDia = p.HorasMaximasPorDia,
                    DuracionBloque = p.DuracionBloque,
                    PeriodoAcademicoId = periodoActivo.Id
                }).ToList()
            };

            await _cursoRepository.AgregarCursoAsync(nuevoCurso);
            return nuevoCurso;
        }

        public async Task<IEnumerable<CompetenciasCursoDto?>> ObtenerPorIdAsync(int id)
        {
            var curso = await _cursoRepository.ObtenerPorIdAsync(id);
            if (curso == null) return Enumerable.Empty<CompetenciasCursoDto>();

            return curso.Competencias.Select(c => new CompetenciasCursoDto
            {
                Id = c.Id,
                NombreCompetencia = c.Nombre,
                NombreCurso = curso.Nombre,
                CursoId = curso.Id,
                GradoId = curso.GradoId,
                NombreGrado = curso.Grado?.Nombre ?? "N/A",
            }).ToList();
        }

        public async Task<PageResponseDto<CursoCompetenciaDto>> ObtenerTodosAsync(int pagina, int cantidad)
        {
            var periodoActivo = await _periodo.ObtenerPeriodoAcademicoActivo();
            var cursos = await _cursoRepository.ObtenerCursosAsync();
            var totalRegistros = cursos.Count();

            var items = cursos
                .OrderBy(c => c.GradoId)
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToList();

            var totalPagina = (int)Math.Ceiling(totalRegistros / (double)cantidad);

            var cursosDto = items.Select(curso => new CursoCompetenciaDto
            {
                Id = curso.Id,
                Nombre = curso.Nombre,
                NombreAula = curso.Grado?.Nombre ?? "Sin Grado",
                GradoId = curso.GradoId,
                HorasSemanales = curso.PlanEstudios.FirstOrDefault(p => p.PeriodoAcademicoId == periodoActivo!.Id)?.HorasSemanales ?? 0,
                Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                {
                    Id = comp.Id,
                    Nombre = comp.Nombre,
                }).ToList()
            }).ToList();

            return new PageResponseDto<CursoCompetenciaDto>
            {
                Items = cursosDto,
                TotalRegistros = totalRegistros,
                PaginaActual = pagina,
                TotalPaginas = totalPagina
            };
        }

        public async Task ActualizarNombreAsync(int id, CursoActualizarDto dto)
        {
            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(id);
            if (cursoExiste is null) throw new Exception("El curso no existe");

            cursoExiste.Nombre = dto.Nombre;
            await _cursoRepository.ActualizarCursoAsync(cursoExiste);
        }

        public async Task<IEnumerable<CursoCompetenciaDto>> ObtenerPorGrado(int gradoId)
        {
            var cursos = await _cursoRepository.ObtenerPorGrado(gradoId);

            return cursos.Select(curso => new CursoCompetenciaDto
            {
                Id = curso.Id,
                Nombre = curso.Nombre,
                NombreAula = curso.Grado?.Nombre ?? "Sin Grado",
                GradoId = curso.GradoId,
                Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                {
                    Id = comp.Id,
                    Nombre = comp.Nombre,
                }).ToList()
            }).ToList();
        }

        public async Task<IEnumerable<CursoCompetenciaDto>> ObtenerPorGradoSeccionAsyn(int gradoId, int seccionId, int periodoId)
        {
            var cursos = await _cursoRepository.ObtenerPorGrado(gradoId);

            var horasAsignadas = await _cursoRepository.ObtenerPorGradoSeccionAsync(gradoId, seccionId, periodoId);

            var listaDto = new List<CursoCompetenciaDto>();

            foreach (var curso in cursos)
            {
                var planes = await _planEstudioRepository.GetAllAsync();
                var planDelCurso = planes.FirstOrDefault(p => p.CursoId == curso.Id);

                int horasTotalesDelPlan = planDelCurso?.HorasSemanales ?? 0;
                int horasOcupadas = horasAsignadas.TryGetValue(curso.Id, out int ocupadas) ? ocupadas : 0;
                int horasRestantes = horasTotalesDelPlan - horasOcupadas;

                listaDto.Add(new CursoCompetenciaDto
                {
                    Id = curso.Id,
                    Nombre = curso.Nombre,
                    NombreAula = curso.Grado?.Nombre ?? "Sin Grado",
                    GradoId = curso.GradoId,
                    HorasSemanales = horasTotalesDelPlan,
                    HorasRestantes = Math.Max(0, horasRestantes),
                    Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                    {
                        Id = comp.Id,
                        Nombre = comp.Nombre,
                    }).ToList()
                });
            }

            return listaDto;
        }
    }
}