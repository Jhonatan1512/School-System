using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface ICalificacionRepository
    {
        Task<Calificacion> RegistrarCalificacion(Calificacion calificacion); 
        Task<Calificacion> ActualizarCalificacionAsync(Calificacion calificacion);
        Task<Calificacion?> ObtenerCalificacionExistente(int detalleMatriculaId, int competenciaId, int trimestreId);

    } 
}
