// Cliente.cs
// Esta clase representa a un cliente que solicita un pedido.
// Contiene los datos del cliente y la dirección, con una posible referencia adicional.
namespace MiCadeteria.Models
{
    public class Cliente
    {
        // Propiedades públicas con set para permitir la creación desde JSON
        public string Nombre { get; set; }  // Nombre del cliente
        public string Direccion { get; set; }  // Dirección principal
        public string Telefono { get; set; }  // Teléfono del cliente
        public string DatosReferenciaDireccion { get; set; } // Referencia adicional de la dirección

        // Constructor vacío necesario para que la API pueda deserializar JSON
        public Cliente() { }

        // Constructor con parámetros opcional
        public Cliente(string nombre, string direccion, string telefono, string datosReferencia)
        {
            Nombre = nombre;
            Direccion = direccion;
            Telefono = telefono;
            DatosReferenciaDireccion = datosReferencia;
        }

        // Método para devolver la dirección completa del cliente
        // Incluye la referencia si existe
        public string VerDireccionCompleta()
        {
            // Si no hay referencia, devuelve solo la dirección
            return string.IsNullOrEmpty(DatosReferenciaDireccion) ? Direccion : $"{Direccion} ({DatosReferenciaDireccion})";
        }

        // Método opcional para mostrar info completa del cliente
        public string VerDatosCliente()
        {
            return $"Nombre: {Nombre}, Tel: {Telefono}, Dirección: {VerDireccionCompleta()}";
        }
    }
}
