using System.Collections.Generic;

namespace MiCadeteria.Models
{
    // Esta clase simula la "base de datos" en memoria.
    // Es estática, por lo que todos los controladores y métodos pueden acceder a los mismos datos.
    public static class DataStore
    {
        // Lista de cadetes
        public static List<Cadete> Cadetes { get; set; } = new List<Cadete>();

        // Lista de pedidos
        public static List<Pedido> Pedidos { get; set; } = new List<Pedido>();

        //id autoincremental de los pedidos para no tener problemas al usar el post y no saber el numero de pedido
        private static int _nextPedidoId = 1; // contador interno de pedidos

        // Objeto Cadeteria general
        public static Cadeteria CadeteriaInfo { get; set; } = new Cadeteria
        {
            Nombre = "Cadetería Express",
            Telefono = "123456789"
        };

        // Constructor estático: se ejecuta una vez cuando se accede a la clase por primera vez
        static DataStore()
        {
            // Inicializar algunos cadetes de ejemplo
            Cadetes.Add(new Cadete { Id = 1, Nombre = "Juan Pérez", Direccion = "Calle Falsa 123", Telefono = "111111111" });
            Cadetes.Add(new Cadete { Id = 2, Nombre = "María Gómez", Direccion = "Av. Siempre Viva 456", Telefono = "222222222" });

            // Inicializar algunos pedidos de ejemplo usando el método
            AgregarPedido(new Pedido
            {
                Observaciones = "Dejar en portería",
                Cliente = new Cliente { Nombre = "Cliente 1", Direccion = "Calle A 123", Telefono = "333333333" },
                Estado = EstadoPedido.Pendiente
            });

            AgregarPedido(new Pedido
            {
                Observaciones = "Llamar al llegar",
                Cliente = new Cliente { Nombre = "Cliente 2", Direccion = "Calle B 456", Telefono = "444444444" },
                Estado = EstadoPedido.Pendiente
            });

            CadeteriaInfo.Cadetes = Cadetes;
            CadeteriaInfo.Pedidos = Pedidos;
        }
        
        // Nuevo método para manejar IDs automáticos
        public static Pedido AgregarPedido(Pedido pedido)
        {
            pedido.Numero = _nextPedidoId++; // asigna y luego incrementa el contador
            Pedidos.Add(pedido);
            return pedido;
        }
    }
}
