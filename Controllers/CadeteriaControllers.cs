using Microsoft.AspNetCore.Mvc;

//Herencia de ControllerBase. Esto es obligatorio para un controlador API sin vistas. Nos da acceso a métodos como Ok(), NotFound(), BadRequest(), etc.

//[ApiController] Permite que ASP.NET Core valide automáticamente los parámetros, el binding de JSON y maneje respuestas automáticas de errores.

//[Route("api/[controller]")] Define la ruta base de todos los endpoints dentro de este controlador.[controller] se reemplaza por el nombre de la clase sin Controller.Ejemplo: CadeteriaController → /api/cadeteria.
namespace MiCadeteria.Models
{
    // Indica que esta clase es un controlador de API
    [ApiController]

    // Define la ruta base del controlador
    // [controller] se reemplaza automáticamente por el nombre del controlador sin "Controller"
    // En este caso: "cadeteria"
    [Route("api/[controller]")]
    public class CadeteriaController : ControllerBase
    {
        // Aquí dentro vamos a agregar los endpoints GET, POST y PUT
        // Este método devuelve todos los pedidos
        [HttpGet("pedidos")]
        public IActionResult GetPedidos()
        {
            // Accedemos directamente a la lista de pedidos en memoria
            var pedidos = DataStore.Pedidos;

            // Devolvemos 200 OK con la lista de pedidos en formato JSON
            return Ok(pedidos);
        }

        // Este método devuelve todos los cadetes
        [HttpGet("cadetes")]
        public IActionResult GetCadetes()
        {
            var cadetes = DataStore.Cadetes;
            return Ok(cadetes);
        }
    }
}