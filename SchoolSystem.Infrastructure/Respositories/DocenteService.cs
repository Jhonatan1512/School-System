using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class DocenteService : IDocenteService
    {
        private readonly ApplicationDbContext _context;
        public DocenteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocenteDto>> GetAllsync()
        {
            var datos = from docente in _context.Docentes
                        join usuarios in _context.Users
                        on docente.UsuarioId equals usuarios.Id
                        where docente.EsActivo == true
                        select new DocenteDto
                        {
                            Id = docente.Id,
                            Nombres = docente.Nombres,
                            Apellidos = docente.Apellidos,
                            Dni = docente.Dni,
                            Email = usuarios.Email,
                        };
            return await datos.ToListAsync();
        }
        
        public async Task<DocenteDto> GetByDniAsync(string dni)
        {
            var datos = from docente in _context.Docentes
                        join usuarios in _context.Users
                        on docente.UsuarioId equals usuarios.Id
                        where docente.Dni == dni 
                        select new DocenteDto
                        {
                            Id = docente.Id,
                            Nombres = docente.Nombres,
                            Apellidos = docente.Apellidos,
                            Dni = docente.Dni,
                            Email = usuarios.Email,
                        };
            return await datos.FirstOrDefaultAsync();
        } 
    }
}
