// Cadeteria.cs
// Esta clase representa la cadetería, que administra cadetes y pedidos.
// Contiene métodos para agregar cadetes, asignar pedidos y generar informes.
using System.Collections.Generic;
using System.Linq;

namespace MiCadeteria.Models
{
    public class Cadeteria
    {
        public string Nombre { get; set; }  // Nombre de la cadetería
        public string Telefono { get; set; } // Teléfono de la cadetería

        // Listas de cadetes y pedidos cargadas desde archivos JSON
        public List<Cadete> Cadetes { get; set; } = new List<Cadete>();
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();

        private const decimal ValorPedido = 500m; // Monto fijo por pedido entregado

        // Constructor vacío para deserialización
        public Cadeteria() { }

        // Constructor opcional
        public Cadeteria(string nombre, string telefono)
        {
            Nombre = nombre;
            Telefono = telefono;
        }

        // Agrega un cadete a la lista si no existe uno con el mismo Id
        public void AgregarCadete(Cadete c)
        {
            if (!Cadetes.Any(x => x.Id == c.Id))
                Cadetes.Add(c);
        }

        // Buscar cadete por Id
        public Cadete BuscarCadete(int id)
        {
            return Cadetes.FirstOrDefault(x => x.Id == id);
        }

        // Dar de alta un nuevo pedido
        public void AltaPedido(Pedido p)
        {
            Pedidos.Add(p);
        }

        // Asignar un cadete a un pedido
        public bool AsignarCadeteAPedido(int idCadete, int numeroPedido)
        {
            var pedido = Pedidos.FirstOrDefault(x => x.Numero == numeroPedido);
            var cadete = BuscarCadete(idCadete);

            if (pedido == null || cadete == null) return false;

            // ⚡ Ahora guardamos solo el Id del cadete
            pedido.AsignarCadete(idCadete);
            pedido.CambiarEstado(EstadoPedido.Asignado);

            return true;
        }

        // Calcular el monto a cobrar de un cadete por pedidos entregados
        public decimal JornalACobrar(int idCadete)
        {
            // ⚡ Comparamos IdCadete en lugar de objeto Cadete
            return Pedidos
                   .Where(p => p.IdCadete == idCadete && p.Estado == EstadoPedido.Entregado)
                   .Count() * ValorPedido;
        }

        // Generar informe final de la jornada
        public List<string> InformeFinalJornada()
        {
            List<string> informe = new List<string>();

            foreach (var c in Cadetes)
            {
                decimal monto = JornalACobrar(c.Id);
                int cantidad = Pedidos.Count(p => p.IdCadete == c.Id && p.Estado == EstadoPedido.Entregado);

                informe.Add($"Cadete: {c.Nombre}, Envíos entregados: {cantidad}, Monto ganado: ${monto}");
            }

            int totalEnvios = Pedidos.Count(p => p.Estado == EstadoPedido.Entregado);
            decimal totalGanado = Cadetes.Sum(c => JornalACobrar(c.Id));
            double promedio = Cadetes.Count > 0 ? Cadetes.Average(c => Pedidos.Count(p => p.IdCadete == c.Id && p.Estado == EstadoPedido.Entregado)) : 0;

            informe.Add($"\nTotal envíos: {totalEnvios}");
            informe.Add($"Total ganado por todos los cadetes: ${totalGanado}");
            informe.Add($"Promedio de envíos por cadete: {promedio:F2}");

            return informe;
        }
    }
}
