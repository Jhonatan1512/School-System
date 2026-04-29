using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
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
        Task<Dictionary<int, int>> ObtenerPorGradoSeccionAsync(int gradoId, int seccionId, int periodoId); 
        Task<List<Curso>> ObtenerPorGrado(int gradoId); 
        Task<bool> ExisteCursoPorNombreYGrado(string nombre, int gradoId);
        Task<bool> ExistePlanPoPerido(TipoJornada jornada);
        Task EliminarCursoAsync(int id);
    }
}
 