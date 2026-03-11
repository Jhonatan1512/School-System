using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IPeriodoAcademicoService
    {
        Task<PeriodoAcademico> CrearPeriodoAcademicoAsync(PeriodoAcademicoDto dto);
    }
}
