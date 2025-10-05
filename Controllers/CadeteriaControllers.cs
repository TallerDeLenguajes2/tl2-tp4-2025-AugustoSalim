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

            // // Validamos que no exista un pedido con el mismo número en DataStore
            // if (DataStore.Pedidos.Any(p => p.Numero == pedido.Numero))
            //     return BadRequest($"Ya existe un pedido con el número {pedido.Numero}.");

                // Validar que no exista un pedido exactamente igual
            bool existeDuplicado = DataStore.Pedidos.Any(p =>
                p.Observaciones == pedido.Observaciones &&
                p.Cliente.Nombre == pedido.Cliente.Nombre &&
                p.Cliente.Direccion == pedido.Cliente.Direccion &&
                p.Cliente.Telefono == pedido.Cliente.Telefono &&
                p.Cliente.DatosReferenciaDireccion == pedido.Cliente.DatosReferenciaDireccion
            );

            if (existeDuplicado)
                return BadRequest("Ya existe un pedido idéntico.");

            // Agregamos el pedido a la lista de pedidos en memoria
            // DataStore.Pedidos es una lista estática, así que cualquier cambio se refleja en todos los endpoints
            var nuevoPedido = DataStore.AgregarPedido(pedido);

            // Retornamos 201 Created indicando que el recurso se creó correctamente
            // Explicación de cada parámetro de CreatedAtAction:
            // 1. nameof(GetPedidos) -> referencia al método GET que puede devolver el pedido o la lista de pedidos
            // 2. new { id = pedido.Numero } -> parámetros de la ruta para ubicar el recurso recién creado (en caso de GET con id)
            // 3. pedido -> el objeto que se devuelve en la respuesta JSON
            return CreatedAtAction(
                nameof(GetPedidos), new { id = nuevoPedido.Numero }, nuevoPedido);
        }

        [HttpPut("asignar")]
        public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete)
        {
            var pedido = DataStore.Pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null)
                return NotFound($"No se encontró un pedido con número {idPedido}.");

            // No permitir asignar si ya fue entregado
            if (pedido.Estado == EstadoPedido.Entregado)
                return BadRequest("No se puede asignar un pedido que ya fue entregado.");

            var cadete = DataStore.Cadetes.FirstOrDefault(c => c.Id == idCadete);
            if (cadete == null)
                return NotFound($"No se encontró un cadete con ID {idCadete}.");

            // Asignamos el cadete
            pedido.AsignarCadete(cadete);

            return Ok(pedido);//
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
            var pedido = DataStore.Pedidos.FirstOrDefault(p => p.Numero == idPedido);
            if (pedido == null)
                return NotFound($"No se encontró un pedido con número {idPedido}.");

            // No permitir cambiar si ya fue entregado
            if (pedido.Estado == EstadoPedido.Entregado)
                return BadRequest("No se puede cambiar el cadete de un pedido ya entregado.");

            var cadete = DataStore.Cadetes.FirstOrDefault(c => c.Id == idNuevoCadete);
            if (cadete == null)
                return NotFound($"No se encontró un cadete con ID {idNuevoCadete}.");

            // No permitir asignar el mismo cadete
            if (pedido.CadeteAsignado != null && pedido.CadeteAsignado.Id == cadete.Id)
                return BadRequest("El pedido ya tiene asignado ese cadete.");

            // Asignamos el nuevo cadete
            pedido.AsignarCadete(cadete);

            return Ok(pedido);
        }


    }
}
