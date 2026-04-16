using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.Application.DTOs;
using SchoolSystem.Application.Interfaces;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IConfiguracionService configuracionService;
        private readonly IConfiguracionRepository configuracionRepository;

        public ConfiguracionController(IConfiguracionService configuracionService, IConfiguracionRepository configuracionRepository)
        {
            this.configuracionService = configuracionService;
            this.configuracionRepository = configuracionRepository;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearAsigncion([FromBody] ConfiguracionGradoSeccion configuracion)
        {
            if(configuracion == null)
            {
                return BadRequest("Todos los campos son obligatorios");
            }
            try
            {
                if (configuracion.GradoId.ToString().IsNullOrEmpty() || 
                    configuracion.SeccionId.ToString().IsNullOrEmpty() || 
                    configuracion.PeriodoacademicoId.ToString().IsNullOrEmpty() ||
                    configuracion.CapacidadMax.ToString().IsNullOrEmpty())
                {
                    return BadRequest("Todos los campos son obligatorios");
                }

                if (configuracion.CapacidadMax <= 0)
                {
                    return BadRequest("La cantidad de cupos deber ser mayores a '0'");
                }

                var resultado = await configuracionService.AgregarConfiguracion(configuracion.GradoId, configuracion.SeccionId, configuracion.CapacidadMax);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ConfiguracionDetalleDto>>> getAll()
        {
            try
            {
                var detalle = await configuracionService.GetDeatilAsync();
                if(detalle is null || !detalle.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron Configuraciones de cupos"});
                }

                return Ok(detalle);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("grado/{gradoId:int}/seccion/{seccionId:int}")]
        public async Task<ActionResult<ConfiguracionDetalleDto>> ObtenerDetalleCuposAula(int gradoId, int seccionId)
        {
            try
            {
                var detalle = await configuracionService.ObtenerPorGradoSeccionAsync(gradoId, seccionId);
                if(detalle is null)
                {
                    return NotFound("Esta aula no tiene cupos definidos");
                }
                return Ok(detalle);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var configuracionExiste = await configuracionRepository.GetconfiguracionById(id);
                if(configuracionExiste is null)
                {
                    return NotFound();
                }

                await configuracionRepository.EliminarConfiguracion(id);
                return NoContent();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] ConfiguracionGradoSecionDto data)
        {
            if (data == null)
            {
                return BadRequest("Todos los campos son obligatorios");
            }
            if (data.CapacidadMax <= 0)
            {
                return BadRequest("La cantidad de cupos deber ser mayores a '0'");
            }
            try
            {
                await configuracionService.ActualizarAsync(id, data);
                return NoContent();

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("grado/{gradoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetByGrado(int gradoId)
        {
            try
            {
                var result = await configuracionService.DetallePorgrado(gradoId);
                return Ok(result);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
