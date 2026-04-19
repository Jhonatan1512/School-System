using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface ICompetenciaService
    {
        Task<Competencia> CrearCompetenciaAsync(CrearCompetenciaDto dto);
        Task ActualizarCompetenciaAsync(int id, CrearCompetenciaDto dto);
    }
}
