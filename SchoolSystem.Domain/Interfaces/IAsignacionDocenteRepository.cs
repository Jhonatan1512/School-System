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
        Task<bool> ExisteAsignacionAsync(int docenteId, int cursoId, int seccionId, int periodoId);

    }
}
