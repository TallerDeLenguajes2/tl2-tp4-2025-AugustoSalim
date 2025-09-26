// EstadoPedido.cs
// Este archivo define los posibles estados de un pedido.
// Usamos un enum para que solo se puedan usar valores válidos
// y evitar errores de tipeo. En la API, esto hace más fácil
// validar y cambiar el estado de un pedido.
namespace MiCadeteria.Models
{
    public enum EstadoPedido
    {
        Pendiente, // Pedido recién creado, sin asignar a cadete
        Asignado,  // Pedido ya tiene un cadete asignado
        Enviado,   // Pedido en proceso de entrega
        Entregado  // Pedido entregado al cliente
    }
}
