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
    public class CompetenciaController : ControllerBase 
    {
        private readonly ICompetenciaService competenciaService;
        private readonly ICompetenciaRepository competenciaRepository;

        public CompetenciaController(ICompetenciaService competenciaService, ICompetenciaRepository competenciaRepository)
        {
            this.competenciaService = competenciaService;
            this.competenciaRepository = competenciaRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CrearCompetencia(CrearCompetenciaDto dto)
        {
            try
            {
                var result = await competenciaService.CrearCompetenciaAsync(dto);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Actualizar(int id, CrearCompetenciaDto dto)
        {
            try
            {
                await competenciaService.ActualizarCompetenciaAsync(id, dto);
                return Ok(new { mensaje = "Competencia Actualizada" });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                await competenciaRepository.EliminarCompetenciaAsync(id);
                return Ok(new { Mensaje = "Competencia eliminada" });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
