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

            var nuevoPeriodo = new PeriodoAcademico
            {
                Nombre = dto.Nombre,
                EstadoActivo = dto.EstadoActivo,
            };

            await _periodoAcademicoRepository.AgregarPeriodoAsync(nuevoPeriodo);

            return nuevoPeriodo;
        }
    }
}
