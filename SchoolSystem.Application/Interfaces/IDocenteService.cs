using SchoolSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IDocenteService
    {
        Task<DocenteDto?> GetByDniAsync(string dni); 
        Task<PageResponseDto<DocenteDto>> GetAllsync(int pagina, int cantidad); 
        Task<List<DashboardDocenteDto>> ObtenerMiDashboardAsync(string usuarioId);
        Task<DetalleCursoDto> ObtenerDetalleCursoAsync(int docenteId, int cursoId, int seccionId, int periodoId);
        Task ActualizarEstadoAsync(int id, ActualizarEstadoDocenteDto dto);
        Task<DocenteDto?> GetPerfilAsyn(string usuarioId); 
    }
}
 