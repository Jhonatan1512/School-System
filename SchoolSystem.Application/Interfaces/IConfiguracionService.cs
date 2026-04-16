using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
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
        Task<ConfiguracionGradoSeccion> ActualizarAsync(int id, ConfiguracionGradoSecionDto dto);
        Task<List<ConfiguracionDetalleDto>> DetallePorgrado(int gradoId);
    }
}
