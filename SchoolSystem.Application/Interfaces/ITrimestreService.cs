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
        Task<IEnumerable<TrimestreDto>> ObtenerPorPeriodo(int periodoId);
        Task<TrimestreDto> ExtenderTrimestreAsync(int id, TrimestreExtensionDto dto);
    }
}
