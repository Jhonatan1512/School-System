using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Application.DTOs;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class AlumnoRepository : IAlumnoRespository
    {
        private readonly ApplicationDbContext _context;

        public AlumnoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarAlumnoAsync(Alumno alumno)
        {
            _context.Alumnos.Update(alumno);
            await _context.SaveChangesAsync();
        }

        public async Task<Alumno> AgregarAlumnoAsync(Alumno alumno)
        {
            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();
            return alumno; 
        }

        public async Task<Alumno?> ObtenerPorDni(string dni)
        {
            return await _context.Alumnos.FirstOrDefaultAsync(a => a.Dni == dni);
        }
    }
}
