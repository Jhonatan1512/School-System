using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlanEstudioController : ControllerBase
    {
        private readonly IPlaEstudiosService _planEstudioService;
        private readonly IPeriodoAcademicoRepository _periodoAcademicoRepository;

        public PlanEstudioController(IPlaEstudiosService plaEstudiosService,  IPeriodoAcademicoRepository periodoAcademicoRepository)
        {
            _planEstudioService = plaEstudiosService;
            _periodoAcademicoRepository = periodoAcademicoRepository;
        } 

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearPla(CrearPlanEstudioDto dto)
        {
            try
            {
                var result = await _planEstudioService.CrearPlanAsync(dto);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarPlan(int id, ActualizarPlanEstudioDto dto)
        {
            try
            {
                await _planEstudioService.ActualizarPlanAsync(id, dto);
                return Ok(new { mensaje = "Datos actualizados"});
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("lista-paginada")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PageResponseDto<PlanEstudiosDto>>> ObtenerTodos([FromQuery] int pagina = 1, [FromQuery] int cantidad = 20)
        {
            if (pagina < 1) pagina = 1;
            if (cantidad > 20) cantidad = 20;

            var periodActivo = await _periodoAcademicoRepository.ObtenerPeriodoAcademicoActivo();
            if(periodActivo == null)
            {
                return BadRequest("No hay periodo activo");
            }
            try
            {
                var reult = await _planEstudioService.GetPlanEstudiosAsync(pagina, cantidad);
                return Ok(reult);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
