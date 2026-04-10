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
    public class ConfiguracionGradoSeccionService : IConfiguracionService
    {
        private readonly IConfiguracionRepository configuracionRepository;
        private readonly IPeriodoAcademicoRepository periodoAcademicoRepository;
        private readonly IMatriculaRepository matriculaRepository;
        
        public ConfiguracionGradoSeccionService(
            IConfiguracionRepository configuracionRepository, 
            IPeriodoAcademicoRepository periodoAcademicoRepository,
            IMatriculaRepository matriculaRepository)
        {
            this.configuracionRepository = configuracionRepository;
            this.periodoAcademicoRepository = periodoAcademicoRepository;
            this.matriculaRepository = matriculaRepository; 
        }
        public async Task<ConfiguracionGradoSecionDto> AgregarConfiguracion(int gradoId, int seccionId, int capacidad)
        {
            var periodActivo = await periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if(periodActivo is null)
            {
                throw new Exception("No hay periodo acadademico activo");
            }

            var ConfigExiste = await configuracionRepository.ObtenerConfiguracionEspecificaAsync(periodActivo.Id, gradoId, seccionId);
            if( ConfigExiste != null)
            {
                throw new Exception("Esta sección ya tiene cupos definidos");
            }

            var nuevaConfig = new ConfiguracionGradoSeccion
            {
                GradoId = gradoId,
                SeccionId = seccionId,
                PeriodoacademicoId = periodActivo.Id,
                CapacidadMax = capacidad
            };

            var resultado = await configuracionRepository.CrearConfiguracionAsync(nuevaConfig);
            if (resultado)
            {
                return new ConfiguracionGradoSecionDto
                {
                    GradoId = nuevaConfig.GradoId,
                    SeccionId = nuevaConfig.SeccionId,
                    PeriodoacademicoId = nuevaConfig.PeriodoacademicoId,
                    CapacidadMax = nuevaConfig.CapacidadMax
                };
            }

            throw new Exception("Error al guardar la configuración");
        }

        public async Task<List<ConfiguracionDetalleDto>> GetDeatilAsync()
        {
            var periodoActivo = await periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodoActivo is null) throw new Exception("No hay periodo académico activo");

            var configuracion = await configuracionRepository.GetDetailsByPeriodo(periodoActivo.Id);

            var listaDtos = new List<ConfiguracionDetalleDto>();

            foreach (var item in configuracion)
            {
                var totalInscritos = await matriculaRepository.ContarMatrculadosAsync(item.GradoId, item.SeccionId, item.PeriodoacademicoId);

                listaDtos.Add(new ConfiguracionDetalleDto
                {
                    id = item.Id,
                    NombreGrado = item.Grado!.Nombre,
                    NombreSeccion = item.Seccion!.Nombre,
                    PeriodoAcademico = item.PeriodoAcademico!.Nombre,
                    Capacidad = item.CapacidadMax,
                    TotalMatriculados = totalInscritos,
                });
            }

            return listaDtos;
        }

        public async Task<ConfiguracionDetalleDto> ObtenerPorGradoSeccionAsync(int gradoId, int seccionId)
        {
            var periodoActivo = await periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if(periodoActivo is null)
            {
                throw new Exception("No hay periodo académico activo");
            }

            var config = await configuracionRepository.ObtenerConfiguracionEspecificaAsync(gradoId, seccionId, periodoActivo.Id);
            if (config is null) return null!;

            int inscritos = await matriculaRepository.ContarMatrculadosAsync(gradoId, seccionId, periodoActivo.Id);

            return new ConfiguracionDetalleDto
            {
                id = config.Id,
                NombreGrado = config.Grado!.Nombre,
                NombreSeccion = config.Seccion!.Nombre,
                PeriodoAcademico = config.PeriodoAcademico!.Nombre,
                Capacidad = config.CapacidadMax,
                TotalMatriculados = inscritos,
            };
        }
    }
}
