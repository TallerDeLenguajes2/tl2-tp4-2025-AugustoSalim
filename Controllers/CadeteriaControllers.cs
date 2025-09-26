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

        //Este metodo devuelve el informe
        [HttpGet("informe")]
        public IActionResult GetInforme()
        {
            // Obtenemos el informe final de la cadetería
            var informe = DataStore.CadeteriaInfo.InformeFinalJornada();

            // Retornamos 200 OK con el informe en JSON
            return Ok(informe);
        }

        [HttpPost("pedidos")]
        //[FromBody] → se usa cuando los datos vienen en el cuerpo de la solicitud (body) en formato JSON o XML. Se pasan objetos o cosas mas complejas
        //[FromQuery] → se usa cuando los datos vienen en la URL, como parámetros simples (int, string). Ejemplo: /api/cadeteria/asignar?idPedido=3&idCadete=1 .El método C# sería: public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete). se pasan atributos o cosas mas simples como int o string
        public IActionResult AgregarPedido([FromBody] Pedido pedido)
        {
            // Validamos que el objeto pedido no sea nulo y que tenga un cliente asignado
            if (pedido == null || pedido.Cliente == null)
                return BadRequest("El pedido o el cliente no pueden ser nulos.");

            // Validamos que no exista un pedido con el mismo número en DataStore
            if (DataStore.Pedidos.Any(p => p.Numero == pedido.Numero))
                return BadRequest($"Ya existe un pedido con el número {pedido.Numero}.");

            // Agregamos el pedido a la lista de pedidos en memoria
            // DataStore.Pedidos es una lista estática, así que cualquier cambio se refleja en todos los endpoints
            DataStore.Pedidos.Add(pedido);

            // Retornamos 201 Created indicando que el recurso se creó correctamente
            // Explicación de cada parámetro de CreatedAtAction:
            // 1. nameof(GetPedidos) -> referencia al método GET que puede devolver el pedido o la lista de pedidos
            // 2. new { id = pedido.Numero } -> parámetros de la ruta para ubicar el recurso recién creado (en caso de GET con id)
            // 3. pedido -> el objeto que se devuelve en la respuesta JSON
            return CreatedAtAction(
                nameof(GetPedidos), new { id = pedido.Numero }, pedido);
        }

        [HttpPut("asignar")]
        public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete) //se recibe los id desde la url
        {
            // Buscamos el pedido en la lista de pedidos
            var pedido = DataStore.Pedidos.FirstOrDefault(p => p.Numero == idPedido);

            // Buscamos el cadete en la lista de cadetes
            var cadete = DataStore.Cadetes.FirstOrDefault(c => c.Id == idCadete);

            // Si no encontramos pedido o cadete, devolvemos 404
            if (pedido == null)
                return NotFound($"No se encontró un pedido con número {idPedido}.");
            if (cadete == null)
                return NotFound($"No se encontró un cadete con id {idCadete}.");

            // Asignamos el cadete al pedido
            pedido.AsignarCadete(cadete);

            // Retornamos 200 OK con el pedido actualizado en JSON
            return Ok(pedido);
        }
        [HttpPut("cambiarEstadoPedido")]
        //Validación automática:Si alguien envía un valor que no está en el enum, ASP.NET Core devuelve 400 Bad Request automáticamente.Por ejemplo, /cambiarEstadoPedido?idPedido=3&nuevoEstado=Enviado → sería rechazado, porque Enviado no existe en el enum.Más seguro:Ya no necesitamos string.IsNullOrWhiteSpace porque no se puede enviar un valor inválido.
        public IActionResult CambiarEstadoPedido([FromQuery] int idPedido, [FromQuery] EstadoPedido nuevoEstado)
        {
            var pedido = DataStore.Pedidos.FirstOrDefault(p => p.Numero == idPedido);

            if (pedido == null)
                return NotFound($"No se encontró un pedido con número {idPedido}.");

            // Cambiamos el estado usando directamente el enum
            pedido.CambiarEstado(nuevoEstado);

            return Ok(pedido);
        }
        // PUT: /api/cadeteria/cambiarCadetePedido?idPedido=3&idNuevoCadete=2
        [HttpPut("cambiarCadetePedido")]
        public IActionResult CambiarCadetePedido([FromQuery] int idPedido, [FromQuery] int idNuevoCadete)
        {
            // 1️⃣ Buscamos el pedido por su número
            var pedido = DataStore.Pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null)
            {
                // Si no existe, devolvemos 404 Not Found con mensaje
                return NotFound($"No se encontró un pedido con número {idPedido}.");
            }

            // 2️⃣ Buscamos el cadete por su ID
            var cadete = DataStore.Cadetes.FirstOrDefault(c => c.Id == idNuevoCadete);
            if (cadete == null)
            {
                // Si no existe, devolvemos 404 Not Found con mensaje
                return NotFound($"No se encontró un cadete con ID {idNuevoCadete}.");
            }

            // 3️⃣ Asignamos el nuevo cadete al pedido
            pedido.AsignarCadete(cadete);

            // 4️⃣ Devolvemos el pedido actualizado con 200 OK
            return Ok(pedido);
        }

    }
}
