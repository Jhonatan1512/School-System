using SchoolSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IAsignacionDocenteService
    {
        Task<AsignacionDocenteDto> AsignarCursoAsync(AsignacionDocenteCreateDto dto);
    }
}
