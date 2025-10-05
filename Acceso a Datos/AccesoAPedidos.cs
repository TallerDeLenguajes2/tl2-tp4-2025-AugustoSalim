using System.Text.Json; // Para trabajar con JSON (leer y escribir)
using MiCadeteria.Models; // Para usar la clase Pedido

namespace MiCadeteria.AccesoADatos
{
    public class AccesoADatosPedidos
    {
        // 🔒 'private readonly' significa:
        // - 'private': solo puede usarse dentro de esta clase.
        // - 'readonly': su valor se asigna UNA sola vez (en la declaración o en el constructor)
        //   y luego no se puede modificar.
        // Lo usamos para proteger la ruta del archivo de posibles cambios accidentales.
        private readonly string rutaArchivo = "Data/Pedidos.json";

        // ✅ Método que obtiene (lee) todos los pedidos desde el archivo JSON
        public List<Pedido> Obtener()
        {
            // Si el archivo NO existe todavía, devolvemos una lista vacía
            if (!File.Exists(rutaArchivo))
            {
                return new List<Pedido>();
            }

            // Leemos todo el contenido del archivo en formato texto
            string json = File.ReadAllText(rutaArchivo);

            // Si el archivo existe pero está vacío, también devolvemos una lista vacía
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Pedido>();
            }

            // 'JsonSerializerOptions' nos deja configurar cómo se interpreta el JSON
            // En este caso decimos que NO distinga entre mayúsculas y minúsculas
            // (por ejemplo, acepta "Numero" o "numero" como válidos)
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 'JsonSerializer.Deserialize' convierte el texto JSON en una lista de objetos Pedido
            // Si algo falla, devolvemos una lista vacía para evitar errores
            return JsonSerializer.Deserialize<List<Pedido>>(json, opciones) ?? new List<Pedido>();
        }

        // ✅ Método que guarda (escribe) todos los pedidos en el archivo JSON
        public void Guardar(List<Pedido> pedidos)
        {
            // Opciones para que el JSON quede "bonito", con indentación
            var opciones = new JsonSerializerOptions { WriteIndented = true };

            // Convertimos la lista de pedidos a formato JSON (texto)
            string json = JsonSerializer.Serialize(pedidos, opciones);

            // Nos aseguramos de que la carpeta 'Data' exista antes de escribir
            Directory.CreateDirectory("Data");

            // Sobrescribimos (o creamos) el archivo Pedidos.json con el nuevo contenido
            File.WriteAllText(rutaArchivo, json);
        }
    }
}
