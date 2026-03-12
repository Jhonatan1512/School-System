using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface ITrimestreRepository
    {
        Task<Trimestre> CrearTrimestreAsync(Trimestre trimestre);
        Task<Trimestre?> ObtenerTrimestreActioPorPeriodo(int periodoAcademicoId);
        Task<IEnumerable<Trimestre>> ObtenerPorPeriodo(int periodoAcademicoId);
        Task<Trimestre?> ObtenerPorIdAsync(int id);
        Task ActualizarTrimestreAsync(Trimestre trimestre);
    }
}
