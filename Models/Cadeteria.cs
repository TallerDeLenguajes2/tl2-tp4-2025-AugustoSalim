// Cadeteria.cs
// Esta clase representa la cadetería, que administra cadetes y pedidos.
// Contiene métodos para agregar cadetes, asignar pedidos y generar informes.
using System.Collections.Generic;
using System.Linq;

namespace MiCadeteria.Models
{
    public class Cadeteria
    {
        // Nombre y teléfono de la cadetería
        public string Nombre { get; set; }
        public string Telefono { get; set; }

        // Listas públicas de cadetes y pedidos
        // En la Web API, estas listas pueden ser compartidas con la clase DataStore
        public List<Cadete> Cadetes { get; set; } = new List<Cadete>();
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();

        private const decimal ValorPedido = 500m; // Monto fijo por pedido entregado

        // Constructor vacío necesario para Web API
        public Cadeteria() { }

        // Constructor con parámetros opcional
        public Cadeteria(string nombre, string telefono)
        {
            Nombre = nombre;
            Telefono = telefono;
        }

        // Método para agregar un cadete a la cadetería
        public void AgregarCadete(Cadete c)
        {
            // Solo agregamos si no existe un cadete con el mismo Id
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

        // Asignar un cadete a un pedido específico
        public bool AsignarCadeteAPedido(int idCadete, int numeroPedido)
        {
            var pedido = Pedidos.FirstOrDefault(x => x.Numero == numeroPedido);
            var cadete = BuscarCadete(idCadete);

            if (pedido == null || cadete == null) return false; // Retorna false si no existe pedido o cadete

            pedido.AsignarCadete(cadete);
            pedido.CambiarEstado(EstadoPedido.Asignado); // Actualiza el estado
            return true; // Retorna true si se asignó correctamente
        }

        // Calcular el monto a cobrar de un cadete por pedidos entregados
        public decimal JornalACobrar(int idCadete)
        {
            var cadete = BuscarCadete(idCadete);
            if (cadete == null) return 0;

            // Contamos los pedidos entregados asignados a este cadete y multiplicamos por el valor
            return Pedidos
                   .Where(p => p.CadeteAsignado == cadete && p.Estado == EstadoPedido.Entregado)
                   .Count() * ValorPedido;
        }

        // Generar un informe final de la jornada
        public List<string> InformeFinalJornada()
        {
            List<string> informe = new List<string>();

            // Información por cadete
            foreach (var c in Cadetes)
            {
                decimal monto = JornalACobrar(c.Id);
                int cantidad = Pedidos.Count(p => p.CadeteAsignado == c && p.Estado == EstadoPedido.Entregado);
                informe.Add($"Cadete: {c.Nombre}, Envíos entregados: {cantidad}, Monto ganado: ${monto}");
            }

            // Información global de la jornada
            int totalEnvios = Pedidos.Count(p => p.Estado == EstadoPedido.Entregado);
            decimal totalGanado = Cadetes.Sum(c => JornalACobrar(c.Id));
            double promedio = Cadetes.Count > 0 ? Cadetes.Average(c => Pedidos.Count(p => p.CadeteAsignado == c && p.Estado == EstadoPedido.Entregado)) : 0;

            informe.Add($"\nTotal envíos: {totalEnvios}");
            informe.Add($"Total ganado por todos los cadetes: ${totalGanado}");
            informe.Add($"Promedio de envíos por cadete: {promedio:F2}");

            return informe;
        }
    }
}
