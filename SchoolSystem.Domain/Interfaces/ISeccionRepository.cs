using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface ISeccionRepository
    {
        Task AgregarSeccionAsync(Seccion seccion);
        Task<IEnumerable<Seccion>> ObtenerTodosAsync();
        Task<Seccion?> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(Seccion seccion);
    }
}
