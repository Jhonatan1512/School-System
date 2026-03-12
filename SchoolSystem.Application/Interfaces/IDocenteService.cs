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
        Task<IEnumerable<DocenteDto>> GetAllsync();
        Task<List<DashboardDocenteDto>> ObtenerMiDashboardAsync(string usuarioId);
        
    }
}
