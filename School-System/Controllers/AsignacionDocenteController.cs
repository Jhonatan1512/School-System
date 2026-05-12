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
    public class AsignacionDocenteController : ControllerBase 
    {
        private readonly IAsignacionDocenteService _asignacionDocenteService;
        private readonly IAsignacionDocenteRepository _asignacionDocenteRepository;

        public AsignacionDocenteController(IAsignacionDocenteService asignacionDocenteService, IAsignacionDocenteRepository asignacionDocenteRepository)
        {
            _asignacionDocenteService = asignacionDocenteService;
            _asignacionDocenteRepository = asignacionDocenteRepository;
        }
         
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AsignarCurso([FromBody] AsignacionDocenteCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Todos los campos son oblatorios");
            }
            try
            {
                var resultado = await _asignacionDocenteService.AsignarCursoAsync(dto);
                return Ok(new
                {
                    mensaje = "Docente asignado al curso",
                    data = resultado
                });
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("lista-paginada")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> getAll([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            if (pagina < 1) pagina = 1;
            if(cantidad > 20) cantidad = 20;
            try
            {
                var datos = await _asignacionDocenteService.obtenerDocentesAsignadosAsync(pagina, cantidad);
                return Ok(datos);


            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActualizarAsignacion(int id, AsignacionDocenteDto dto)
        {
            if(dto == null)
            {
                return BadRequest("Todos los campos son oblatorios");
            }
            try
            {
                await _asignacionDocenteService.ActualizarAsignacionAsync(id, dto);
                return Ok(new { mensaje = "Se actualizo la asignación del docente"});
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
                await _asignacionDocenteRepository.EliminarAsignacionAsync(id);
                return NoContent();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gradoId/{gradoId:int}/seccionId/{seccionId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> getByGradoSeccion(int gradoId, int seccionId)
        {
            try
            {
                var result = await _asignacionDocenteService.ObtenerPorGradoSeccionAsync(gradoId, seccionId);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("docente/{dni}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> obtenerPorDni(string dni)
        {
            try
            {
                var result = await _asignacionDocenteService.ObtenerPorDniDocenteAsync(dni);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
