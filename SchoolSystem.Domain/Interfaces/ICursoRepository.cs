using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface ICursoRepository
    {
        Task AgregarCursoAsync(Curso curso);
        Task<Curso?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Curso>> ObtenerCursosAsync();
        Task ActualizarCursoAsync(Curso curso);
        Task<List<Curso>> ObtenerPorGrado(int gradoId);
    }
}
