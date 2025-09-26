// Cadete.cs
// Esta clase representa a un cadete, es decir, una persona que entrega pedidos.
// En la Web API, los objetos de esta clase pueden enviarse y recibirse como JSON.
namespace MiCadeteria.Models
{
    public class Cadete
    {
        // Propiedades del cadete
        // public set permite que la API cree y actualice objetos desde JSON
        public int Id { get; set; }  // Identificador único del cadete
        public string Nombre { get; set; }  // Nombre completo
        public string Direccion { get; set; }  // Dirección del cadete
        public string Telefono { get; set; }  // Teléfono de contacto

        // Constructor vacío requerido por Web API para deserializar objetos JSON
        public Cadete() { }

        // Constructor con parámetros opcional
        public Cadete(int id, string nombre, string direccion, string telefono)
        {
            Id = id;
            Nombre = nombre;
            Direccion = direccion;
            Telefono = telefono;
        }

        // Si quisieras mostrar info del cadete en consola o logs, se podría usar
        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Nombre}, Dirección: {Direccion}, Tel: {Telefono}";
        }
    }
}
