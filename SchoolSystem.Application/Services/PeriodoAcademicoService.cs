using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSystem.Application.Services
{
    public class PeriodoAcademicoService : IPeriodoAcademicoService
    {
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;
        public PeriodoAcademicoService(IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            _periodoAcademicoRepository = periodoAcademicoRepository;
        }

        public async Task<PeriodoAcademico> ActualizarPeriodo(PeriodoacademicoActualizarDto dto)
        { 
            var periodoEditar = await _periodoAcademicoRepository.GetByIdAsync(dto.Id);
            if (periodoEditar is null) throw new Exception("Periodo no encontrado");
             
            if (dto.EstadoActivo)
            {
                var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
                if (periodoActivo != null && periodoActivo.Id != dto.Id)
                {
                    periodoActivo.EstadoActivo = false;
                    await _periodoAcademicoRepository.ActualizarPeriodoAsync(periodoActivo);
                }
            }

            periodoEditar.Nombre = dto.Nombre;
            periodoEditar.EstadoActivo = dto.EstadoActivo;

            if(dto.FechaCierre != DateTime.MinValue)
            {
                periodoEditar.FechaCierre = dto.FechaCierre;
            }

            await _periodoAcademicoRepository.ActualizarPeriodoAsync(periodoEditar);
            return periodoEditar;
        }

        public async Task<PeriodoAcademico> CrearPeriodoAcademicoAsync(PeriodoAcademicoDto dto)
        {
            if (dto.EstadoActivo)
            {
                var periodoAnterior = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
                 
                if (periodoAnterior != null) 
                {
                    periodoAnterior.EstadoActivo = false;
                    await _periodoAcademicoRepository.ActualizarPeriodoAsync(periodoAnterior);
                }
            }

            int anioactual = DateTime.UtcNow.Year;

            var nuevoPeriodo = new PeriodoAcademico
            {
                Nombre = dto.Nombre,
                EstadoActivo = dto.EstadoActivo,
                FechaInicio = DateTime.UtcNow,
                FechaCierre = new DateTime(anioactual, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            };

            await _periodoAcademicoRepository.AgregarPeriodoAsync(nuevoPeriodo);

            return nuevoPeriodo;
        }
    }
}
