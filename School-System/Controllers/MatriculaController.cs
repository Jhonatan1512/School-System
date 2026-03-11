using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatriculaService _matriculaService;
        public MatriculaController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        //POST :api/matricula
        [HttpPost]
        public async Task<IActionResult> AgregarMatricula([FromBody] MatriculaDto dto)
        {
            try
            {
                await _matriculaService.AgregarMatriculaDetallerAsync(dto);
                return Ok(new { mensaje = "Alumno matriculado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //PUT :api/matricula/id
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarMatricula(int id, [FromBody] ActualizarMatriculaDto dto)
        {
            try
            {
                await _matriculaService.ActualizarMatricula(id, dto);
                return Ok(new { mensaje = "Se actualizo la matricula del estudiante" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET :api/matricula
        [HttpGet("grado/{gradoId}/seccion/{seccionId}")]
        public async Task<IActionResult> ObtenerPorAula(int gradoId, int seccionId)
        {
            try
            {
                var alumnos = await _matriculaService.ObtenerMatriculadosPorAula(gradoId, seccionId);
                return Ok(alumnos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
