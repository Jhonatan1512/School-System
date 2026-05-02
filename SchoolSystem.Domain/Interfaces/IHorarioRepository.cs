using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IHorarioRepository
    {
        Task<bool> ExisteCruce(int docenteId, string dia, int horaLectiva, int periodoId);
        Task<bool> ExisteCruceSeccion(int seccionId, string dia, int horaLectivaId, int periodoId);
        Task InsertarRangoAsync(IEnumerable<Horario> horarios);
        Task LimpiarHorariosPeriodoAsync(int periodoId);
        Task<List<Horario>> ObtenerPorGradoSeccionPeriodo(int gradoId, int seccionId, int periodoId); 
        Task<List<Horario>> ObtenerPorDocentePeriodoAsync(int docenteId,  int periodoId);
    }
}
