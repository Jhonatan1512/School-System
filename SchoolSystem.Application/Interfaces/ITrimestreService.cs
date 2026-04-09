using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface ITrimestreService
    {
        Task<TrimestreDto> CrearTrimestreAsync(TrimestreCreadoDto dto);
        Task<IEnumerable<GetTrimestresDto>> ObtenerPorPeriodo();
        Task<TrimestreDto> ExtenderTrimestreAsync(int id, TrimestreExtensionDto dto); 
        Task<TrimestreDto> ActualizarTrimestreAsync(int id, ActualizarTrimestreDto dto);
    }
}
