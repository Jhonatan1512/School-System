using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface ICompetenciaRepository
    {
        Task CrearCompetenciaAsync(Competencia competencia);
        Task EditarCompetenciaAsync(Competencia competencia);
        Task EliminarCompetenciaAsync(int id);
        Task<Competencia?> BuscarPorIdAsync(int id);
    }
}
