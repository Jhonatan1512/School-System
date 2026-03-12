using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrimestreController : ControllerBase
    {
        private readonly ITrimestreService _trimestreService;
        public TrimestreController(ITrimestreService trimestreService)
        {
            _trimestreService = trimestreService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearTrimestre([FromBody] TrimestreCreadoDto dto)
        {
            try
            {
                var resultado = await _trimestreService.CrearTrimestreAsync(dto);
                return CreatedAtAction(nameof(ObtenerPoPeriodo), new { periodoId = resultado.PeriodoAcademicoId }, resultado);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("periodo/{periodoId}")]
        public async Task<IActionResult> ObtenerPoPeriodo(int periodoId)
        {
            try
            {
                var trimestres = await _trimestreService.ObtenerPorPeriodo(periodoId);
                return Ok(trimestres);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/extender")]
        public async Task<IActionResult> ExtenderTrimestre(int id, [FromBody] TrimestreExtensionDto dto)
        {
            try
            {
                var resultado = await _trimestreService.ExtenderTrimestreAsync(id, dto);
                return Ok(resultado);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
