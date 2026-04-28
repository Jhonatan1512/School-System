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
    public class PlanEstudioService : IPlaEstudiosService
    {
        private readonly IPlanEstudioRepository _planEstudioRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IPeriodoAcademicoRepository _periodo;
        public PlanEstudioService(IPlanEstudioRepository planEstudioRepository, 
            ICursoRepository cursoRepository, 
            IPeriodoAcademicoRepository periodo)
        {
            _planEstudioRepository = planEstudioRepository;   
            _cursoRepository = cursoRepository;
            _periodo = periodo;
        }
        public async Task ActualizarPlanAsync(ActualizarPlanEstudioDto dto)
        {
            var planExiste = await _planEstudioRepository.ObtenerPorIdAsync(dto.Id);
            if (planExiste == null)
                throw new Exception("El plan no existe");

            if (dto.HorasSemanales <= 0)
                throw new Exception("Las horas semanales debe ser mayor a 0");

            if (dto.DuracionBloque <= 0)
                throw new Exception("La duración del bloque debe ser mayor a 0.");

            if (dto.HorasMaximasPorDia < dto.DuracionBloque)
                throw new Exception("Las horas máximas por día no pueden ser menores que la duración del bloque.");

            if (dto.HorasSemanales < dto.HorasMaximasPorDia)
                throw new Exception("Las horas semanales no pueden ser menores que las horas máximas por día.");

            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(dto.CursoId);
            if (cursoExiste == null)
            {
                throw new Exception("El curso no existe");
            }

            if(planExiste.CursoId != dto.CursoId || planExiste.Jornada != dto.Jornada)
            {
                bool existePlan = await _planEstudioRepository.ExistePlanAsync(dto.CursoId, dto.Jornada);
                if (existePlan)
                {
                    throw new Exception("Ya existe un plan de estudios para este curso en la jornada seleccionada.");
                }
            }

            planExiste.CursoId = dto.CursoId;
            planExiste.Jornada = dto.Jornada;
            planExiste.HorasMaximasPorDia = dto.HorasMaximasPorDia;
            planExiste.HorasSemanales = dto.HorasSemanales;
            planExiste.DuracionBloque = dto.DuracionBloque;

            await _planEstudioRepository.ActualizarAsync(planExiste);
        }

        public async Task<PlanEstudiosDto> CrearPlanAsync(CrearPlanEstudioDto dto)
        {
            if (dto.HorasSemanales <= 0)
                throw new Exception("Las horas semanales debe ser mayor a 0");
             
            if (dto.DuracionBloque <= 0)
                throw new Exception("La duración del bloque debe ser mayor a 0.");

            if (dto.HorasMaximasPorDia < dto.DuracionBloque)
                throw new Exception("Las horas máximas por día no pueden ser menores que la duración del bloque.");

            if (dto.HorasSemanales < dto.HorasMaximasPorDia)
                throw new Exception("Las horas semanales no pueden ser menores que las horas máximas por día.");

            var cursoExiste = await _cursoRepository.ObtenerPorIdAsync(dto.CursoId);
            if(cursoExiste == null)
            {
                throw new Exception("El curso no existe");
            }

            var periodoExiste = await _periodo.ObtenerPeriodoAcademicoActivo();
            if (periodoExiste == null)
            {
                throw new Exception("El periodo académico no esta activo");
            }

            bool jornadaDistinta = await _cursoRepository.ExistePlanPoPerido(dto.Jornada);
            if (jornadaDistinta)
            {
                throw new Exception("La jornada que esta intentando registrar es distinta a los demás grados");
            }

            bool existePlan = await _planEstudioRepository.ExistePlanAsync(dto.CursoId, dto.Jornada);
            if (existePlan)
            {
                throw new Exception("Ya existe un plan de estudios para este curso en la jornada seleccionada.");
            }

            var nuevoPlan = new PlanEstudio
            {
                CursoId = dto.CursoId,
                Jornada = dto.Jornada,
                HorasSemanales = dto.HorasSemanales,
                HorasMaximasPorDia = dto.HorasMaximasPorDia,
                DuracionBloque = dto.DuracionBloque,
                PeriodoAcademicoId = periodoExiste.Id,
            };

            var planGuardado = await _planEstudioRepository.CrearAsync(nuevoPlan);

            return new PlanEstudiosDto
            {
                Id = planGuardado.Id,
                CursoId = planGuardado.CursoId,
                NombreCurso = cursoExiste.Nombre, 
                Jornada = planGuardado.Jornada,
                HorasSemanales = planGuardado.HorasSemanales,
                HorasMaximasPorDia = planGuardado.HorasMaximasPorDia,
                DuracionBloque = planGuardado.DuracionBloque
            };
        }
    }
}
