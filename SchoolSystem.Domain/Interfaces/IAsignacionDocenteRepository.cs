using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IAsignacionDocenteRepository
    { 
        Task<List<AsignacionDocente>> ObtenerPorSeccionPeriodoAsync(int seccionId, int periodoId);
        Task<List<AsignacionDocente>> ObtenerAsignacionCompletaDocenteAsync(int docenteId, int periodoId);
        Task<AsignacionDocente> CrearAsignacionAsync(AsignacionDocente dto);
        Task<bool> ExisteAsignacionAsync(int docenteId, int planEstudioId, int seccionId, int periodoId);
        Task<IEnumerable<AsignacionDocente>> GetAllAsync();
        Task ActualizarAsignacionAsync(int id, AsignacionDocente asignacion);
        Task<AsignacionDocente?> ObtenerPorIdAsync(int id);
        Task EliminarAsignacionAsync(int id);
        Task<IEnumerable<AsignacionDocente>> ObtenerPorGradoSeccion(int gradoId, int seccionId);
        Task<IEnumerable<AsignacionDocente>> ObtenerPorPeriodoAsync(int periodoId);
        Task<int> ObtenerHorasCubiertasPlanEstudioAsync(int planEstudioId, int gradoId, int seccionId, int periodoId, int excluirId);
        Task<int> ObtenerHorasTotalesDocenteAsync(int docenteId, int periodoId, int excluirId);
        Task<List<AsignacionDocente>> ObtenerPorDniDocente(string dni);
    }
}