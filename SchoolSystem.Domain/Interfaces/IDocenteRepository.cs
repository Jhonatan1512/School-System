using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IDocenteRepository
    {
        Task<Docente> CrearDocenteAsync(Docente docente);
        Task<Docente?> ObtenerPorDniAsync(string dni);
        Task ActualizarDoncenteAsync(Docente docente); 
        Task<Docente?> ObtenerPorUsuarioAsync(string usuarioId);
    }
}
