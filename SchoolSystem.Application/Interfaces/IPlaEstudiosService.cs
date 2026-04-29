using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IPlaEstudiosService
    {
        Task<List<PlanEstudiosDto>> CrearPlanAsync(CrearPlanEstudioDto dto);
        Task ActualizarPlanAsync(int id, ActualizarPlanEstudioDto dto);
        Task<PageResponseDto<PlanEstudiosDto>> GetPlanEstudiosAsync(int pagina, int cantidad); 
    }
}
