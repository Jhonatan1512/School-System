using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IAlumnoService
    {
        Task<PageResponseDto<AlumnoDto>> GetAll(int pagina, int cantidad); 
        Task<AlumnoDto?> GetByIdAsync(int id);
        Task<AlumnoDto?> GetByDniAsync(string dni);
        Task<List<DashboardAlumnoDto>> ObtenerMisCursos(string usuarioId);
        Task<DetalleCursoAlumnoDto> ObtenerDetalleCursoAsync(int alumnoId, int cursoId, int periodoId); 
        Task<IEnumerable<AlumnoDto>> AlumnosSeccionAsync(int gradoId, int seccionId, int periodoId);
        Task ActualizarEstadoAsync(int id, ActualizarEstadoDto dto); 
        Task<AlumnoDto?> GetPerfilAsync(string usuarioId);
    }
}
