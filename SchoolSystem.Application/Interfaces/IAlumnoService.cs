using SchoolSystem.Application.DTOs;
using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IAlumnoService
    {
        Task<IEnumerable<AlumnoDto>> GetAll();
        Task<AlumnoDto?> GetByIdAsync(int id);
        Task<AlumnoDto?> GetByDniAsync(string dni);
    }
}
