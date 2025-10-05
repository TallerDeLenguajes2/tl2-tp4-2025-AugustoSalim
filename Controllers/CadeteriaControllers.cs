using Microsoft.AspNetCore.Mvc;
using MiCadeteria.AccesoADatos;
using MiCadeteria.Models;
using System.Linq;

//Herencia de ControllerBase. Esto es obligatorio para un controlador API sin vistas. 
//Nos da acceso a métodos como Ok(), NotFound(), BadRequest(), etc.

//[ApiController] Permite que ASP.NET Core valide automáticamente los parámetros, 
//el binding de JSON y maneje respuestas automáticas de errores.

//[Route("api/[controller]")] Define la ruta base de todos los endpoints dentro de este controlador.
//[controller] se reemplaza por el nombre de la clase sin "Controller".
//Ejemplo: CadeteriaController → /api/cadeteria.

namespace MiCadeteria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CadeteriaController : ControllerBase
    {
        // ============================================================================================
        //  🔹 ACCESO A DATOS
        // ============================================================================================

        // Las variables 'readonly' indican que solo se pueden asignar una vez (en el constructor).
        // Esto asegura que las instancias de acceso a datos no se modifiquen en tiempo de ejecución.
        private readonly AccesoADatosCadeteria accesoCadeteria;
        private readonly AccesoADatosCadetes accesoCadetes;
        private readonly AccesoADatosPedidos accesoPedidos;

        // Objetos principales del sistema cargados desde los archivos JSON.
        private Cadeteria cadeteria;
        private List<Cadete> cadetes;
        private List<Pedido> pedidos;

        // ============================================================================================
        //  🔹 CONSTRUCTOR
        // ============================================================================================

        // Este constructor se ejecuta automáticamente cuando se crea una instancia del controlador.
        // Aquí cargamos toda la información desde los archivos JSON usando las clases de acceso a datos.
        public CadeteriaController()
        {
            accesoCadeteria = new AccesoADatosCadeteria();
            accesoCadetes = new AccesoADatosCadetes();
            accesoPedidos = new AccesoADatosPedidos();

            // Cargamos los datos persistidos desde los archivos JSON.
            cadeteria = accesoCadeteria.Obtener();
            cadetes = accesoCadetes.Obtener();
            pedidos = accesoPedidos.Obtener();
        }

        // ============================================================================================
        //  🔹 ENDPOINTS GET (Obtener información)
        // ============================================================================================

        // Devuelve todos los pedidos en formato JSON
        [HttpGet("pedidos")]
        public IActionResult GetPedidos()
        {
            return Ok(pedidos);
        }

        // Devuelve todos los cadetes disponibles
        [HttpGet("cadetes")]
        public IActionResult GetCadetes()
        {
            return Ok(cadetes);
        }

        // Devuelve la información general de la cadetería
        [HttpGet("informe")]
        public IActionResult GetInforme()
        {
            // A futuro, podríamos generar un informe real con estadísticas, totales, etc.
            return Ok(cadeteria);
        }

        // ============================================================================================
        //  🔹 ENDPOINT POST (Agregar pedido)
        // ============================================================================================

        [HttpPost("pedidos")]
        //[FromBody] indica que los datos se leen desde el cuerpo (body) de la solicitud en formato JSON.
        public IActionResult AgregarPedido([FromBody] Pedido pedido)
        {
            // Validamos que el pedido tenga cliente y datos válidos.
            if (pedido == null || pedido.Cliente == null)
                return BadRequest("El pedido o el cliente no pueden ser nulos.");

            // Validamos que no exista un pedido exactamente igual.
            bool existeDuplicado = pedidos.Any(p =>
                p.Observaciones == pedido.Observaciones &&
                p.Cliente.Nombre == pedido.Cliente.Nombre &&
                p.Cliente.Direccion == pedido.Cliente.Direccion &&
                p.Cliente.Telefono == pedido.Cliente.Telefono &&
                p.Cliente.DatosReferenciaDireccion == pedido.Cliente.DatosReferenciaDireccion
            );

            if (existeDuplicado)
                return BadRequest("Ya existe un pedido idéntico.");

            // Generamos un nuevo Id de pedido autoincremental.
            pedido.Numero = pedidos.Count > 0 ? pedidos.Max(p => p.Numero) + 1 : 1;

            // Agregamos el pedido a la lista.
            pedidos.Add(pedido);

            // 💾 Guardamos los cambios en el archivo JSON inmediatamente.
            accesoPedidos.Guardar(pedidos);

            // CreatedAtAction crea una respuesta HTTP 201 (Created),
            // e incluye la ruta del recurso recién creado.
            return CreatedAtAction(nameof(GetPedidos), new { id = pedido.Numero }, pedido);
        }

        // ============================================================================================
        //  🔹 ENDPOINT PUT (Asignar pedido a cadete)
        // ============================================================================================

        [HttpPut("asignar")]
        //[FromQuery] indica que los parámetros vienen por URL, por ejemplo:
        // /api/cadeteria/asignar?idPedido=3&idCadete=1
        public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null)
                return NotFound($"No se encontró un pedido con ID {idPedido}.");

            // No se puede asignar un pedido que ya fue entregado
            if (pedido.Estado == EstadoPedido.Entregado)
                return BadRequest("No se puede asignar un pedido que ya fue entregado.");

            var cadete = cadetes.FirstOrDefault(c => c.Id == idCadete);
            if (cadete == null)
                return NotFound($"No se encontró un cadete con ID {idCadete}.");

            // Asignamos el cadete
            pedido.IdCadete = cadete.Id;
            pedido.Estado = EstadoPedido.Asignado;

            // Guardamos los cambios en el JSON
            accesoPedidos.Guardar(pedidos);

            return Ok($"Pedido {idPedido} asignado correctamente al cadete {cadete.Nombre}.");
        }

        // ============================================================================================
        //  🔹 ENDPOINT PUT (Cambiar estado del pedido)
        // ============================================================================================

        [HttpPut("cambiarEstadoPedido")]
        public IActionResult CambiarEstadoPedido([FromQuery] int idPedido, [FromQuery] EstadoPedido nuevoEstado)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null)
                return NotFound($"No se encontró un pedido con ID {idPedido}.");

            // Cambiamos el estado directamente
            pedido.Estado = nuevoEstado;

            // Guardamos los cambios
            accesoPedidos.Guardar(pedidos);

            return Ok(pedido);
        }

        // ============================================================================================
        //  🔹 ENDPOINT PUT (Cambiar cadete asignado)
        // ============================================================================================

        [HttpPut("cambiarCadetePedido")]
        public IActionResult CambiarCadetePedido([FromQuery] int idPedido, [FromQuery] int idNuevoCadete)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null)
                return NotFound($"No se encontró un pedido con ID {idPedido}.");

            // No permitir cambiar si ya fue entregado
            if (pedido.Estado == EstadoPedido.Entregado)
                return BadRequest("No se puede cambiar el cadete de un pedido ya entregado.");

            var nuevoCadete = cadetes.FirstOrDefault(c => c.Id == idNuevoCadete);
            if (nuevoCadete == null)
                return NotFound($"No se encontró un cadete con ID {idNuevoCadete}.");

            // No permitir asignar el mismo cadete
            if (pedido.IdCadete == nuevoCadete.Id)
                return BadRequest("El pedido ya tiene asignado ese cadete.");

            // Asignamos el nuevo cadete
            pedido.IdCadete = nuevoCadete.Id;

            // Guardamos cambios
            accesoPedidos.Guardar(pedidos);

            return Ok($"Pedido {idPedido} reasignado al cadete {nuevoCadete.Nombre}.");
        }
    }
}
