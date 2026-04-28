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

        public PlanEstudioController(IPlaEstudiosService plaEstudiosService)
        {
            _planEstudioService = plaEstudiosService;
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
        public async Task<ActionResult> ActualizarPlan(ActualizarPlanEstudioDto dto)
        {
            try
            {
                await _planEstudioService.ActualizarPlanAsync(dto);
                return Ok(new { mensaje = "Datos actualizados"});
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
