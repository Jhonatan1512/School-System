using SchoolSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface ICalificacionService
    {
        Task<List<CalificacionDto>> RegistroMasivo(int docenteId, List<CalificacionCreateDto> list);
    }
}
 