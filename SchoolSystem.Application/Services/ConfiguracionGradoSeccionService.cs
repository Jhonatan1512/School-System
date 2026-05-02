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

        public async Task<ConfiguracionGradoSeccion> ActualizarAsync(int id, ConfiguracionGradoSecionDto dto)
        {
            var configuracion = await configuracionRepository.GetconfiguracionById(id);
            if(configuracion == null)
            {
                throw new Exception("No existe este registrio de confifuración");
            }

            var cantidadMatriculados = await matriculaRepository.ContarMatrculadosAsync(configuracion.GradoId, configuracion.SeccionId, configuracion.PeriodoacademicoId);
            if(dto.CapacidadMax <= cantidadMatriculados)
            {
                throw new Exception($"No se puede reducir los cupos, ya hay {cantidadMatriculados} alumnos inscritos");
            }

            configuracion.CapacidadMax = dto.CapacidadMax;

            await configuracionRepository.ActualizarConfiguracionAsync(configuracion);
            return configuracion;
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

        public async Task<List<ConfiguracionDetalleDto>> DetallePorgrado(int gradoId)
        {
            var periodActivo = await periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodActivo == null) throw new Exception("No hay periodos activos");

            var connfiguracion = await configuracionRepository.GetDetailsByPeriodo(periodActivo.Id);

            var configuracionFiltrada = connfiguracion.Where(c => c.GradoId == gradoId).ToList();

            var listaDto = new List<ConfiguracionDetalleDto>();

            foreach(var l in configuracionFiltrada)
            {
                //var totalInscritos = await matriculaRepository.ContarMatrculadosAsync(l.GradoId, l.SeccionId, l.PeriodoacademicoId);

                if(l.VacantesOcupadas < l.CapacidadMax)
                {
                    listaDto.Add(new ConfiguracionDetalleDto
                    {
                        id = l.Id,
                        NombreGrado = l.Grado!.Nombre,
                        NombreSeccion = l.Seccion!.Nombre,
                        GradoId = l.Grado.Id,
                        PeriodoAcademico = l.PeriodoAcademico!.Nombre,
                        SeccionId = l.Seccion.Id,
                        PeriodoId = l.PeriodoAcademico.Id,
                        Capacidad = l.CapacidadMax,
                        VacantesOcupadas = l.VacantesOcupadas,
                    });
                }
            }

            return listaDto;
        }

        public async Task<List<ConfiguracionDetalleDto>> GetDeatilAsync()
        {
            var periodoActivo = await periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if (periodoActivo is null) throw new Exception("No hay periodo académico activo");

            var configuracion = await configuracionRepository.GetDetailsByPeriodo(periodoActivo.Id);

            var listaDtos = new List<ConfiguracionDetalleDto>();

            foreach (var item in configuracion)
            {
                //var totalInscritos = await matriculaRepository.ContarMatrculadosAsync(item.GradoId, item.SeccionId, item.PeriodoacademicoId);

                listaDtos.Add(new ConfiguracionDetalleDto
                {
                    id = item.Id,
                    NombreGrado = item.Grado!.Nombre,
                    NombreSeccion = item.Seccion!.Nombre,
                    GradoId = item.Grado.Id,
                    PeriodoAcademico = item.PeriodoAcademico!.Nombre,
                    SeccionId = item.Seccion.Id,
                    PeriodoId = item.PeriodoAcademico.Id,
                    Capacidad = item.CapacidadMax,
                    VacantesOcupadas = item.VacantesOcupadas,
                });
            }

            return listaDtos.OrderBy(x => x.GradoId).ThenBy(x => x.NombreSeccion).ToList();
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

            //int inscritos = await matriculaRepository.ContarMatrculadosAsync(gradoId, seccionId, periodoActivo.Id);

            return new ConfiguracionDetalleDto
            {
                id = config.Id,
                NombreGrado = config.Grado!.Nombre,
                NombreSeccion = config.Seccion!.Nombre,
                PeriodoAcademico = config.PeriodoAcademico!.Nombre,
                Capacidad = config.CapacidadMax,
                VacantesOcupadas = config.VacantesOcupadas,
            };
        }
    }
}
