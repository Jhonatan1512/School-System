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
    public class TrimestreService : ITrimestreService
    {
        public readonly ITrimestreRepository _trimestreRepository;
        public TrimestreService(ITrimestreRepository trimestreRepository)
        {
            _trimestreRepository = trimestreRepository;
        }
        public async Task<TrimestreDto> CrearTrimestreAsync(TrimestreCreadoDto dto)
        {
            var nuevoTrimestre = new Trimestre
            {
                Nombre = dto.Nombre,
                FechaInicio = dto.FechaInicio,
                FechaCierre = dto.FechaCierre,
                PeriodoAcademicoId = dto.PeriodoAcademicoId,
                EstadoActivo = dto.EstadoActivo,
            };
            var creado = await _trimestreRepository.CrearTrimestreAsync(nuevoTrimestre);

            return new TrimestreDto
            {
                Id = creado.Id,
                Nombre = creado.Nombre,
                FechaInicio = creado.FechaInicio,
                FechaCierre = creado.FechaCierre,
                EstadoActivo = creado.EstadoActivo,
                PeriodoAcademicoId = creado.PeriodoAcademicoId
            };
        }

        public async Task<TrimestreDto> ExtenderTrimestreAsync(int id, TrimestreExtensionDto dto)
        {
            var trimestre = await _trimestreRepository.ObtenerPorIdAsync(id);
            if (trimestre == null) throw new Exception("El trimestre no estiste");

            trimestre.FechaCierre = dto.NuevaFechaCierre;
            trimestre.EstadoActivo = true;

            await _trimestreRepository.ActualizarTrimestreAsync(trimestre);

            return new TrimestreDto
            {
                Id = trimestre.Id,
                Nombre = trimestre.Nombre,
                FechaInicio = trimestre.FechaInicio,
                FechaCierre = trimestre.FechaCierre,
                EstadoActivo = trimestre.EstadoActivo,
                PeriodoAcademicoId = trimestre.PeriodoAcademicoId
            };
            
        }

        public async Task<IEnumerable<TrimestreDto>> ObtenerPorPeriodo(int periodoId)
        {
            var trimestre = await _trimestreRepository.ObtenerPorPeriodo(periodoId);

            return trimestre.Select(t => new TrimestreDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                FechaInicio = t.FechaInicio,
                FechaCierre = t.FechaCierre,
                EstadoActivo = t.EstadoActivo,
                PeriodoAcademicoId = t.PeriodoAcademicoId
            });
        }
    }
}
