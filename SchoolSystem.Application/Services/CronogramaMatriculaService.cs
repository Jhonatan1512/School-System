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

        public async Task ActualizarCronogramaAsync(int id, ActualizarCronogramaDto dto)
        {
            var exiteRegistro = await _cronogramaMatriculaRepository.ObtenerPorIdAsync(id);
            if (exiteRegistro == null)
            {
                throw new Exception("El registro no existe en la BD");
            }

            if(dto.NuevaFechaHoraInicio > dto.NuevaFechaHoraCierre)
            {
                throw new Exception("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            exiteRegistro.FechaHoraInicio = dto.NuevaFechaHoraInicio;
            exiteRegistro.FechaHoraFin = dto.NuevaFechaHoraCierre;

            await _cronogramaMatriculaRepository.ActualizarCronogramaAsync(exiteRegistro);
        }

        public async Task<CronogramaMatricula> CreateAsync(CronogramaMatriculaDto dto) 
        {
            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();

            var existeAsignacion = await _cronogramaMatriculaRepository.ObtenerActivoPorGradoAsync(dto.GradoId, periodoActivo!.Id);

            if(existeAsignacion != null)
            {
                throw new Exception("Ya existe un cronograma para este grado en el presente periodo");
            }

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

        public async Task<IEnumerable<GetCronogramaDto>> GetAllAsync()
        {
            var periodoActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();

            var getCronograma = await _cronogramaMatriculaRepository.ObtenerTodosAsync();

            var cronogramasDto = getCronograma.Select(cronograma => new GetCronogramaDto
            {
                Id = cronograma.Id,
                PeriodoId = periodoActivo!.Id,
                NombrePeriodo = periodoActivo.Nombre,
                GradoId = cronograma.GradoId,
                NombreGrado = cronograma.Grado!.Nombre,
                FechaHoraInicio = cronograma.FechaHoraInicio,
                FechaHoraCierre = cronograma.FechaHoraFin,
                EstadoActivo = cronograma.EstadoActivo 
            });

            return cronogramasDto;
        }
    }
}
