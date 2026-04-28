using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface ICursoService
    {
        Task<Curso> CrearCursoCompetenciaAsync(CrearCursoComptenciasDto dto);
        Task<PageResponseDto<CursoCompetenciaDto>> ObtenerTodosAsync(int pagina, int cantidad);
        Task<bool> ActualizarCursoCompetenciaAsync(int id, CursoCompetenciaDto dto);
        Task<IEnumerable<CompetenciasCursoDto?>> ObtenerPorIdAsync(int id);
        Task ActualizarNombreAsync(int id, CursoActualizarDto dto);
        Task<IEnumerable<CursoCompetenciaDto>> ObtenerPorGrado(int gradoId); 
        Task<IEnumerable<CursoCompetenciaDto>> ObtenerPorGradoSeccionAsyn(int gradoId, int seccionId, int periodoId);
    }
}