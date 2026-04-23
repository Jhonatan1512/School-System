using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrimestreController : ControllerBase
    {
        private readonly ITrimestreService _trimestreService;
        private readonly ITrimestreRepository trimestreRepository;

        public TrimestreController(ITrimestreService trimestreService, ITrimestreRepository trimestreRepository)
        {
            _trimestreService = trimestreService;
            this.trimestreRepository = trimestreRepository;
        }

        [HttpPost] 
        [Authorize(Roles = "Admin")]
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

        [HttpGet("periodo")]
        public async Task<IActionResult> ObtenerPoPeriodo()
        {
            try
            {
                var trimestres = await _trimestreService.ObtenerPorPeriodo();
                return Ok(trimestres);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}/extender")]
        [Authorize(Roles = "Admin")]
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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Actualizar(int id, ActualizarTrimestreDto dto)
        {
            try
            {
                var resultado = await _trimestreService.ActualizarTrimestreAsync(id, dto);
                return Ok(new { mensaje = "Trimestre actualizado"});
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var trimestreExiste = await trimestreRepository.ObtenerPorIdAsync(id);
                if(trimestreExiste is null)
                {
                    return NotFound("El trimestre no existe");
                }

                await trimestreRepository.EliminarAsync(id);
                return Ok(new { mensaje = "Trimestre eliminado"});

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
