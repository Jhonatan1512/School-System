using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using SchoolSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Respositories
{
    public class DocenteRepository : IDocenteRepository
    {
        private readonly ApplicationDbContext _context;

        public DocenteRepository(ApplicationDbContext context)
        {
            _context = context;
        } 

        public async Task ActualizarDoncenteAsync(Docente docente) 
        {
            _context.Docentes.Update(docente);
            await _context.SaveChangesAsync();  
        } 

        public async Task<Docente> CrearDocenteAsync(Docente docente)
        {
            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();
            return docente;
        }

        public async Task<Docente?> ObtenerPorDniAsync(string dni)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Dni == dni);
        }

        public async Task<Docente?> ObtenerPorId(int id)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<Docente?> ObtenerPorUsuarioAsync(string usuarioId)
        {
            return await _context.Docentes.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId && u.EsActivo);
        }

        public async Task<Docente?> ObtenerActivoAsync(int id)
        {
            return await _context.Docentes.Where(d => d.Id == id && d.EsActivo).FirstOrDefaultAsync();
        }

        public async Task<Docente?> PerfilDocenteAsync(string usuarioId)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
        }

        
    }
}
