using SchoolSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IAsignacionDocenteService
    {
        Task<List<AsignacionDocenteDto>> AsignarCursoAsync(AsignacionDocenteCreateDto dto);
        Task<PageResponseDto<GetAsignación>> obtenerDocentesAsignadosAsync(int pagina, int cantidad);
        Task ActualizarAsignacionAsync(int id, AsignacionDocenteDto dto);
        Task<IEnumerable<GetAsignación>> ObtenerPorGradoSeccionAsync(int gradoId, int seccionId);
    } 
}
 