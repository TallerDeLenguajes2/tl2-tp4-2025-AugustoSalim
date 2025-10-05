using Microsoft.AspNetCore.Mvc;
using MiCadeteria.AccesoADatos;
using MiCadeteria.Models;
using System.Linq;
using System.Text.Json;
using System.IO;

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

        private readonly AccesoADatosCadeteria accesoCadeteria;
        private readonly AccesoADatosCadetes accesoCadetes;
        private readonly AccesoADatosPedidos accesoPedidos;

        private Cadeteria cadeteria;
        private List<Cadete> cadetes;
        private List<Pedido> pedidos;

        // ============================================================================================
        //  🔹 CONSTRUCTOR
        // ============================================================================================

        public CadeteriaController()
        {
            accesoCadeteria = new AccesoADatosCadeteria();
            accesoCadetes = new AccesoADatosCadetes();
            accesoPedidos = new AccesoADatosPedidos();

            cadeteria = accesoCadeteria.Obtener();
            cadetes = accesoCadetes.Obtener();
            pedidos = accesoPedidos.Obtener();

            // 🔹 Inicializamos listas si vienen nulas para evitar problemas
            cadeteria.Cadetes ??= new List<Cadete>();
            cadeteria.Pedidos ??= new List<Pedido>();

            // 🔹 Sincronizamos cadetes y pedidos cargados desde JSON
            cadeteria.Cadetes = cadetes;
            cadeteria.Pedidos = pedidos;

            // 🔹 Guardamos la cadetería inicial en caso de que no exista
            GuardarCadeteriaJson();
        }

        // ============================================================================================
        //  🔹 MÉTODO AUXILIAR: Guardar cadetería en data/cadeteria.json
        // ============================================================================================
        private void GuardarCadeteriaJson()
        {
            try
            {
                string carpeta = Path.Combine("data");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string ruta = Path.Combine(carpeta, "cadeteria.json");
                string json = JsonSerializer.Serialize(cadeteria, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(ruta, json);
            }
            catch
            {
                // Ignoramos errores de guardado para no romper la API
            }
        }

        // ============================================================================================
        //  🔹 ENDPOINTS GET
        // ============================================================================================

        [HttpGet("pedidos")]
        public IActionResult GetPedidos() => Ok(pedidos);

        [HttpGet("cadetes")]
        public IActionResult GetCadetes() => Ok(cadetes);

        [HttpGet("informe")]
        public IActionResult GetInforme() => Ok(cadeteria);

        // ============================================================================================
        //  🔹 ENDPOINT POST (Agregar cadete)
        // ============================================================================================

        [HttpPost("cadetes")]
        public IActionResult AgregarCadete([FromBody] Cadete cadete)
        {
            if (cadete == null) return BadRequest("Cadete nulo");

            if (cadetes.Any(c => c.Id == cadete.Id))
                return BadRequest("Ya existe un cadete con ese Id");

            cadetes.Add(cadete);
            cadeteria.Cadetes = cadetes;

            accesoCadetes.Guardar(cadetes);
            GuardarCadeteriaJson();

            return CreatedAtAction(nameof(GetCadetes), new { id = cadete.Id }, cadete);
        }

        // ============================================================================================
        //  🔹 ENDPOINT POST (Agregar pedido)
        // ============================================================================================

        [HttpPost("pedidos")]
        public IActionResult AgregarPedido([FromBody] Pedido pedido)
        {
            if (pedido == null || pedido.Cliente == null)
                return BadRequest("El pedido o el cliente no pueden ser nulos.");

            bool existeDuplicado = pedidos.Any(p =>
                p.Observaciones == pedido.Observaciones &&
                p.Cliente.Nombre == pedido.Cliente.Nombre &&
                p.Cliente.Direccion == pedido.Cliente.Direccion &&
                p.Cliente.Telefono == pedido.Cliente.Telefono &&
                p.Cliente.DatosReferenciaDireccion == pedido.Cliente.DatosReferenciaDireccion
            );

            if (existeDuplicado)
                return BadRequest("Ya existe un pedido idéntico.");

            pedido.Numero = pedidos.Count > 0 ? pedidos.Max(p => p.Numero) + 1 : 1;
            pedido.IdCadete = null;

            pedidos.Add(pedido);
            cadeteria.Pedidos = pedidos;

            accesoPedidos.Guardar(pedidos);
            GuardarCadeteriaJson();

            return CreatedAtAction(nameof(GetPedidos), new { id = pedido.Numero }, pedido);
        }

        // ============================================================================================
        //  🔹 ENDPOINT PUT (Asignar pedido a cadete)
        // ============================================================================================

        [HttpPut("asignar")]
        public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null) return NotFound($"No se encontró un pedido con ID {idPedido}.");

            if (pedido.Estado == EstadoPedido.Entregado)
                return BadRequest("No se puede asignar un pedido que ya fue entregado.");

            var cadete = cadetes.FirstOrDefault(c => c.Id == idCadete);
            if (cadete == null) return NotFound($"No se encontró un cadete con ID {idCadete}.");

            pedido.IdCadete = cadete.Id;
            pedido.Estado = EstadoPedido.Asignado;

            accesoPedidos.Guardar(pedidos);
            GuardarCadeteriaJson();

            return Ok($"Pedido {idPedido} asignado correctamente al cadete {cadete.Nombre}.");
        }

        // ============================================================================================
        //  🔹 ENDPOINT PUT (Cambiar estado del pedido)
        // ============================================================================================

        [HttpPut("cambiarEstadoPedido")]
        public IActionResult CambiarEstadoPedido([FromQuery] int idPedido, [FromQuery] EstadoPedido nuevoEstado)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null) return NotFound($"No se encontró un pedido con ID {idPedido}.");

            pedido.Estado = nuevoEstado;

            accesoPedidos.Guardar(pedidos);
            GuardarCadeteriaJson();

            return Ok(pedido);
        }

        // ============================================================================================
        //  🔹 ENDPOINT PUT (Cambiar cadete asignado)
        // ============================================================================================

        [HttpPut("cambiarCadetePedido")]
        public IActionResult CambiarCadetePedido([FromQuery] int idPedido, [FromQuery] int idNuevoCadete)
        {
            var pedido = pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null) return NotFound($"No se encontró un pedido con ID {idPedido}.");

            if (pedido.Estado == EstadoPedido.Entregado)
                return BadRequest("No se puede cambiar el cadete de un pedido ya entregado.");

            var nuevoCadete = cadetes.FirstOrDefault(c => c.Id == idNuevoCadete);
            if (nuevoCadete == null) return NotFound($"No se encontró un cadete con ID {idNuevoCadete}.");

            if (pedido.IdCadete == nuevoCadete.Id)
                return BadRequest("El pedido ya tiene asignado ese cadete.");

            pedido.IdCadete = nuevoCadete.Id;

            accesoPedidos.Guardar(pedidos);
            GuardarCadeteriaJson();

            return Ok($"Pedido {idPedido} reasignado al cadete {nuevoCadete.Nombre}.");
        }
    }
}
