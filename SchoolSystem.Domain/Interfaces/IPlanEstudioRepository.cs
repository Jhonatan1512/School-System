using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IPlanEstudioRepository
    {
        Task<PlanEstudio?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<PlanEstudio>> GetAllAsync();
        Task<IEnumerable<PlanEstudio>> ObtenerPorJornadaAsync(TipoJornada jornada);
        Task<PlanEstudio> CrearAsync(PlanEstudio planEstudio);
        Task ActualizarAsync(PlanEstudio planEstudio);
        Task EliminarAsync(int id);
        Task<bool> ExistePlanAsync(int cursoId, TipoJornada tipoJornada);
    }
}
