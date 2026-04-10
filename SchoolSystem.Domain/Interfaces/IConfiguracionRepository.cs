using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IConfiguracionRepository
    {
        Task<bool> CrearConfiguracionAsync(ConfiguracionGradoSeccion gradoSeccion);
        Task<ConfiguracionGradoSeccion?> ObtenerConfiguracionEspecificaAsync(int periodoId, int gradoId, int seccionId);
        Task<List<ConfiguracionGradoSeccion>> GetDetailsByPeriodo(int periodoId);
        Task EliminarConfiguracion(int id);
        Task<ConfiguracionGradoSeccion?> GetconfiguracionById(int id);
        Task ActualizarConfiguracionAsync(ConfiguracionGradoSeccion data);
    }
}
