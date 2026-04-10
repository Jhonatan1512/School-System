using SchoolSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IConfiguracionService
    {
        Task<ConfiguracionGradoSecionDto> AgregarConfiguracion(int gradoId, int seccionId, int periodoId);
        Task<List<ConfiguracionDetalleDto>> GetDeatilAsync();
        Task<ConfiguracionDetalleDto> ObtenerPorGradoSeccionAsync(int gradoId, int seccionId);
    }
}
