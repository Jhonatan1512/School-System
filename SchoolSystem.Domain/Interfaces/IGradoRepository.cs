using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IGradoRepository 
    {
        Task<Grado> CrearGrado(Grado grado);
        Task<Grado?> ObtenerPorId(int id);
    }
}
