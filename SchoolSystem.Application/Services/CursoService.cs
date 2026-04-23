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
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IGradoRepository _gradoRepository;

        public CursoService(ICursoRepository cursoRepository, IGradoRepository gradoRepository)
        {
            _cursoRepository = cursoRepository; 
            _gradoRepository = gradoRepository;
        }  
          
        public async Task<bool> ActualizarCursoCompetenciaAsync(int id, CursoCompetenciaDto dto) 
        {
            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(id); 
            if (cursoExiste == null) return false;

            cursoExiste.Nombre = dto.Nombre;
            cursoExiste.GradoId = dto.GradoId;
            cursoExiste.HorasSemanales = dto.HorasSemanales;
            cursoExiste.HorasMaximasPorDia = dto.HorasMaximasPorDia;
            cursoExiste.DuracionBloque = dto.DuracionBloque;
            cursoExiste.Prioridad = (PrioridadCurso)dto.Prioridad;

            await _cursoRepository.ActualizarCursoAsync(cursoExiste);
            return true;
        }

        public async Task<Curso> CrearCursoCompetenciaAsync(CursoCompetenciaDto dto)
        {
            var gradoExiste = await _gradoRepository.ObtenerPorId(dto.GradoId);
            if(gradoExiste == null)
            {
                throw new Exception("El grado especificado no existe");
            }

            if(dto.DuracionBloque > dto.HorasMaximasPorDia)
            {
                throw new Exception("La duración de un bloque no puede ser mayor que las horas por día");
            }

            if(dto.HorasMaximasPorDia > dto.HorasSemanales)
            {
                throw new Exception("La duración de horas por día no puede ser mayor que las horas semanales");
            }

            var nuevoCurso = new Curso 
            {
                Nombre = dto.Nombre,
                GradoId = dto.GradoId,
                HorasSemanales = dto.HorasSemanales,
                HorasMaximasPorDia = dto.HorasMaximasPorDia,
                DuracionBloque = dto.DuracionBloque,
                Prioridad = (PrioridadCurso)dto.Prioridad,
                Competencias = dto.Competencias.Select(c => new Competencia
                {
                    Nombre = c.Nombre
                }).ToList()
            };

            await _cursoRepository.AgregarCursoAsync(nuevoCurso);
            return nuevoCurso;
        }

        public async Task<IEnumerable<CompetenciasCursoDto?>> ObtenerPorIdAsync(int id)
        {
            var curso = await _cursoRepository.ObtenerPorIdAsync(id);

            var cursoDto = curso!.Competencias.Select(c => new CompetenciasCursoDto
            {
                Id = c.Id,
                NombreCompetencia = c.Nombre,
                NombreCurso = c.Curso!.Nombre,
                CursoId = curso.Id,
                GradoId = curso.GradoId,
                NombreGrado = curso.Grado!.Nombre,
            }).ToList();
            return cursoDto;
        }

        public async Task<PageResponseDto<CursoCompetenciaDto>> ObtenerTodosAsync(int pagina, int cantidad)
        {
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
                HorasSemanales = curso.HorasSemanales,
                HorasMaximasPorDia = curso.HorasMaximasPorDia,
                Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                {
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

        public async Task ActualuzarAsync(int id, CursoActualizarDto dto)
        {
            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(id);
            if(cursoExiste is null)
            {
                throw new Exception("El curso no existe");
            }

            cursoExiste.Nombre = dto.Nombre;
            await _cursoRepository.ActualizarCursoAsync(cursoExiste);
        }

        public async Task<IEnumerable<CursoCompetenciaDto>> ObtenerPorGrado(int gradoId)
        {
            var gradoExiste = await _gradoRepository.ObtenerPorId(gradoId);
            if(gradoExiste is null)
            {
                throw new Exception("El grado no existe");
            }

            var cursos = await _cursoRepository.ObtenerPorGrado(gradoId);

            var cursosOcupado = await _cursoRepository.ObtenerPorGrado(gradoId);

            var cursosDto = cursos.Select(curso => new CursoCompetenciaDto
            {
                Id = curso.Id,
                Nombre= curso.Nombre,
                NombreAula = curso.Grado?.Nombre ?? "Sin Grado",
                GradoId = curso.GradoId,
                HorasSemanales = curso.HorasSemanales,
                HorasMaximasPorDia = curso.HorasMaximasPorDia,
                Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                {
                    Nombre = comp.Nombre,
                }).ToList()
            }).ToList();

            return cursosDto;
        }

        public async Task<IEnumerable<CursoCompetenciaDto>> ObtenerPorGradoSeccionAsyn(int gradoId, int seccionId, int periodoId)
        {
            var gradoExiste = await _gradoRepository.ObtenerPorId(gradoId);
            if (gradoExiste is null)
            {
                throw new Exception("El grado no existe");
            }

            var cursos = await _cursoRepository.ObtenerPorGrado(gradoId);

            var horasAsignadas = await _cursoRepository.ObtenerPorGradoSeccionAsync(gradoId, seccionId, periodoId);

            var cursosDto = cursos.Select(curso =>
            {
                int horasOcupadas = horasAsignadas.TryGetValue(curso.Id, out int ocupadas) ? ocupadas : 0;
                int horasRestantes = curso.HorasSemanales - horasOcupadas;

                return new CursoCompetenciaDto
                {
                    Id = curso.Id,
                    Nombre = curso.Nombre,
                    NombreAula = curso.Grado!.Nombre ?? "Sin Grado",
                    GradoId = curso.GradoId,
                    HorasSemanales = curso.HorasSemanales,
                    HorasMaximasPorDia = curso.HorasMaximasPorDia,
                    HorasRestantes = horasRestantes < 0 ? 0 : horasRestantes,

                    Competencias = curso.Competencias.Select(comp => new CrearCompetenciaDto
                    {
                        Nombre = curso.Nombre,
                    }).ToList()
                };
            }).ToList();

            return cursosDto;
        }
    }
}
