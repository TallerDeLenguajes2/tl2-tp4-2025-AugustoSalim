// Pedido.cs
// Esta clase representa un pedido de un cliente que será entregado por un cadete.
// Contiene información sobre el pedido, el cliente, el cadete asignado y el estado.
namespace MiCadeteria.Models
{
    public class Pedido
    {
        // Propiedades públicas con set para poder recibir JSON en POST/PUT
        public int Numero { get; set; }                   // Identificador único del pedido
        public string Observaciones { get; set; }         // Observaciones adicionales sobre el pedido
        public Cliente Cliente { get; set; }              // Cliente que solicita el pedido

        // ⚡ Ahora guardamos solo el Id del cadete asignado para simplificar JSON
        // Se usa int? para permitir null (sin cadete asignado)
        public int? IdCadete { get; set; }               // Id del cadete que entregará el pedido (puede ser null)

        public EstadoPedido Estado { get; set; }          // Estado del pedido usando enum directamente

        // Constructor vacío para Web API y deserialización de JSON
        public Pedido() 
        { 
            Estado = EstadoPedido.Pendiente; // Inicialmente siempre pendiente
        }

        // Constructor con parámetros opcional
        public Pedido(int numero, string observaciones, Cliente cliente)
        {
            Numero = numero;
            Observaciones = observaciones;
            Cliente = cliente;
            Estado = EstadoPedido.Pendiente; // Por defecto pendiente
            IdCadete = null;                 // Sin cadete asignado al inicio
        }

        // Método para cambiar el estado del pedido recibiendo directamente el enum
        public void CambiarEstado(EstadoPedido nuevoEstado)
        {
            // ✅ Se asigna directamente el enum, sin ToString()
            // ASP.NET Core serializará automáticamente el enum a JSON como string
            Estado = nuevoEstado;
        }

        // Método para asignar un cadete al pedido (recibe el Id del cadete)
        public void AsignarCadete(int idCadete)
        {
            // Guardamos solo el Id del cadete
            IdCadete = idCadete;
        }

        // Método para quitar el cadete asignado
        public void QuitarCadete()
        {
            IdCadete = null;
        }

        // Método opcional para mostrar información del pedido en consola o logs
        public override string ToString()
        {
            // Nota: como ahora guardamos solo IdCadete, el nombre del cadete se podría buscar aparte si es necesario
            string cadete = IdCadete.HasValue ? $"Cadete ID {IdCadete}" : "Sin asignar";
            return $"Pedido #{Numero}, Estado: {Estado}, Cadete: {cadete}, Cliente: {Cliente?.Nombre}";
        }
    }
}
