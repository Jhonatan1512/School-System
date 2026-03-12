using SchoolSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Domain.Interfaces
{
    public interface IAlumnoRespository
    {
        Task<Alumno> AgregarAlumnoAsync(Alumno alumno);
        Task ActualizarAlumnoAsync(Alumno alumno);
        Task<Alumno?> ObtenerPorDni(string dni);
        Task<Alumno?> ObtenerPorUsuarioAsync(string usuarioId);

    }
}
