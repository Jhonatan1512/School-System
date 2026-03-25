using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Interfaces;
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
        private readonly IGradoSevice _gradoSevice;
        public GradoController(IGradoRepository gradoRepository, IGradoSevice gradoSevice) 
        {
            _gradoRepository = gradoRepository;
            _gradoSevice = gradoSevice;
        }

        //GET :api/grado
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Obtenergrado(int id)
        {
            var grado = await _gradoSevice.GetGradoDetalle(id);
            if (grado == null) return NotFound(new {mensaje = "Grado no entontrado"});
            return Ok(grado);
        }

        //POST :api/grado
        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var grados = await _gradoRepository.GetAllAsync();
            return Ok(grados);
        }
    }
}
