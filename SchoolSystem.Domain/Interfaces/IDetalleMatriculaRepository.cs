using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IDetalleMatriculaRepository
    {
        Task<DetalleMatricula> ObtenerDetallePorId(int id);
    }
}
