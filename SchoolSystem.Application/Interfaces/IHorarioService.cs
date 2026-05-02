using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IHorarioService
    {
        Task<HorarioResultDto> GenerarHorarioAsync(int periodoId);
        Task<List<HorarioSeccionDto>> ObtenerHorariosPorGradoSeccion(int gradoId, int seccionId, int periodoId);
        Task<List<HorarioSeccionDto>> ObtenerPorDocenteAsync(int docenteId, int periodoId);
    }
}
 