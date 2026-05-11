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
        Task<Docente?> ObtenerPorId(int id);
        Task<Docente?> ObtenerActivoAsync(int id); 
        Task<Docente?> PerfilDocenteAsync(string usuarioId);
        Task<AsignacionDocente?> ObtenerAsignacionTutoriaAsync(int docenteId, int periodoId);
        Task<List<Matricula>> ObtenerAlumnosSeccionAsync(int gradoId, int seccionId, int periodoId);
        Task<bool> ValidarTutoriaAlumnoAsync(int docenteId, int alumnoId, int periodoId);
        Task<List<Calificacion>> ObtenerNotasCompletasAsync(int alumnoId, int periodoId);
    }
}
