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
    public class CronogramaMatriculaService : ICronogramaMatriculaService
    {
        private readonly ICronogramaRepository _cronogramaMatriculaRepository;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;

        public CronogramaMatriculaService(ICronogramaRepository cronogramaMatriculaRepository, IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            _cronogramaMatriculaRepository = cronogramaMatriculaRepository;
            _periodoAcademicoRepository = periodoAcademicoRepository;
        }
        public async Task<CronogramaMatricula> CreateAsync(CronogramaMatriculaDto dto)
        {

            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();

            var cronogramaDto = new CronogramaMatricula
            {
                GradoId = dto.GradoId,
                PeriodoAcademicoId = periodoActivo!.Id,
                FechaHoraInicio = dto.FechaHoraInicio,
                FechaHoraFin = dto.FechaHoraCierre,
                EstadoActivo = dto.EstadoActivo,
            };

            await _cronogramaMatriculaRepository.CrearCronogramaAsync(cronogramaDto);

            return cronogramaDto;
        }
    }
}
