// Pedido.cs
// Esta clase representa un pedido de un cliente que será entregado por un cadete.
// Contiene información sobre el pedido, el cliente, el cadete asignado y el estado.
namespace MiCadeteria.Models
{
    public class Pedido
    {
        // Propiedades públicas con set para poder recibir JSON en POST/PUT
        public int Numero { get; set; }  // Identificador único del pedido
        public string Observaciones { get; set; }  // Observaciones adicionales sobre el pedido
        public Cliente Cliente { get; set; }  // Cliente que solicita el pedido
        public Cadete CadeteAsignado { get; set; } // Cadete que entregará el pedido (puede ser null)
        public EstadoPedido Estado { get; set; } // Estado del pedido usando enum

        // Constructor vacío para Web API
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
            CadeteAsignado = null; // Sin cadete asignado al inicio
        }

        // Método para cambiar el estado del pedido
        public void CambiarEstado(EstadoPedido nuevoEstado)
        {
            Estado = nuevoEstado;
        }

        // Método para asignar un cadete al pedido
        public void AsignarCadete(Cadete c)
        {
            CadeteAsignado = c;
        }

        // Método para quitar el cadete asignado
        public void QuitarCadete()
        {
            CadeteAsignado = null;
        }

        // Método opcional para mostrar información del pedido
        public override string ToString()
        {
            string cadete = CadeteAsignado != null ? CadeteAsignado.Nombre : "Sin asignar";
            return $"Pedido #{Numero}, Estado: {Estado}, Cadete: {cadete}, Cliente: {Cliente.Nombre}";
        }
    }
}
