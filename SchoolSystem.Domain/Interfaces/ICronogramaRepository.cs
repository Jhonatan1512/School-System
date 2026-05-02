using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface ICronogramaRepository
    {
        Task CrearCronogramaAsync(CronogramaMatricula cronograma);
        Task ActualizarCronogramaAsync(CronogramaMatricula cronograma);
        Task<CronogramaMatricula?> ObtenerActivoPorGradoAsync(int gradoId, int periodoId);
        Task<IEnumerable<CronogramaMatricula>> ObtenerTodosAsync();
        Task<CronogramaMatricula?> ObtenerPorIdAsync(int id);
    }
}
