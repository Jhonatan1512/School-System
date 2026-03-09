using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Interfaces;

namespace School_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradoController : ControllerBase
    {
        private readonly IGradoRepository _gradoRepository;
        public GradoController(IGradoRepository gradoRepository) 
        {
            _gradoRepository = gradoRepository;
        }

        //GET :api/grado
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtenergrado(int id)
        {
            var grado = await _gradoRepository.ObtenerPorId(id);
            if (grado == null) return NotFound(new {mensaje = "Grado no entontrado"});
            return Ok(grado);
        }

        //POST :api/grado
        [HttpPost]
        public async Task<IActionResult> CrearGrado(Grado grado)
        {
            var nuevoGrado = new Grado
            {
                Nombre = grado.Nombre,
            };

            await _gradoRepository.CrearGrado(nuevoGrado);
            return Ok(new
            {
                mesaje = "Grado creado",
                Id = nuevoGrado.Id,
                Nombre = nuevoGrado.Nombre,
            });
        }
    }
}
