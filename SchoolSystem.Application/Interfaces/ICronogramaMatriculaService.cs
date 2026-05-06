using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface ICronogramaMatriculaService
    {
        Task<CronogramaMatricula> CreateAsync(CronogramaMatriculaDto dto);
        Task<IEnumerable<GetCronogramaDto>> GetAllAsync();
        Task ActualizarCronogramaAsync(int id, ActualizarCronogramaDto dto);


    }
}
