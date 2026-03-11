using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IPeriodoAcademicoRepository
    {
        Task AgregarPeriodoAsync(PeriodoAcademico periodo);
        Task<PeriodoAcademico?> ObtenerPeriodoAcademicoActivo();
        Task ActualizarPeriodoAsync(PeriodoAcademico periodo);
        Task<IEnumerable<PeriodoAcademico>> ObtenerTodosasync();
    }
}
