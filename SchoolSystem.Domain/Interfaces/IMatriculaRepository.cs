using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IMatriculaRepository
    {
        Task AgregaratriculaAsync(Matricula matricula);
        Task<Matricula?> ObtenerPorAlumnoPeriodoAsync(int alumnoId, int periodoId);
        Task ActualizarMatricula(Matricula matricula);
        Task<Matricula?> ObtenerDetallerId(int id);
        Task<List<Matricula>> ObtenerPorAulaAsync(int gradoId, int seccionId, int periodoId);
    }
}
