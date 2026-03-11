using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class AlumnoService : IAlumnoService
    {
        private readonly ApplicationDbContext _context;
        public AlumnoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AlumnoDto>> GetAll()
        {
            var query = from alumno in _context.Alumnos
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        };
            return await query.ToListAsync(); 
        }

        public async Task<AlumnoDto?> GetByDniAsync(string dni)
        {
            var datos = from alumno in _context.Alumnos
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id
                        where alumno.Dni == dni
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        };
            return await datos.FirstOrDefaultAsync();
        }

        public async Task<AlumnoDto?> GetByIdAsync(int id)
        {
            var datos = from alumno in _context.Alumnos
                        join usuario in _context.Users
                        on alumno.UsuarioId equals usuario.Id
                        where alumno.Id == id
                        select new AlumnoDto
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellidos = alumno.Apellidos,
                            Dni = alumno.Dni,
                            FechaNacimiento = alumno.FechaNacimiento,
                            Sexo = alumno.Sexo,
                            Estado = alumno.Estado.ToString(),
                            Email = usuario.Email!
                        };
            return await datos.FirstOrDefaultAsync();
        }
    }
}
