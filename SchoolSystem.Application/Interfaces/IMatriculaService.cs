using SchoolSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Interfaces
{
    public interface IMatriculaService
    {
        Task<bool> AgregarMatriculaDetallerAsync(MatriculaDto dto);
        Task<bool> ActualizarMatricula(int matriculaId, ActualizarMatriculaDto dto);
        Task<List<GetMatriculadosSeccionDto>> ObtenerMatriculadosPorAula(int gradoId, int seccionId);
    }
}
