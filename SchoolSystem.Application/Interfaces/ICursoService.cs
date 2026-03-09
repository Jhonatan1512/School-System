using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface ICursoService
    {
        Task<Curso> CrearCursoCompetenciaAsync(CursoCompetenciaDto dto);
        Task<IEnumerable<CursoCompetenciaDto>> ObtenerTodosAsync();
        Task<bool> ActualizarCursoCompetenciaAsync (int id, CursoCompetenciaDto dto);
    }
}
