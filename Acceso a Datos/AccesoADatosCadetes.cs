using System.Text.Json; // Para leer y convertir JSON
using MiCadeteria.Models; // Para usar la clase Cadete

namespace MiCadeteria.AccesoADatos
{
    public class AccesoADatosCadetes
    {
        // 🔒 Ruta fija (de solo lectura) al archivo de cadetes.
        // Se define como 'readonly' para protegerla de modificaciones accidentales.
        private readonly string rutaArchivo = "Data/Cadetes.json";

        // ✅ Método principal: lee la lista de cadetes desde el archivo JSON
        public List<Cadete> Obtener()
        {
            // Si el archivo todavía no existe (por ejemplo, primera ejecución del programa),
            // devolvemos una lista vacía para evitar errores de lectura.
            if (!File.Exists(rutaArchivo))
            {
                return new List<Cadete>();
            }

            // Leemos todo el contenido del archivo en formato texto (string)
            string json = File.ReadAllText(rutaArchivo);

            // Si el archivo existe pero está vacío, devolvemos lista vacía también.
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Cadete>();
            }

            // Configuramos las opciones del deserializador JSON
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Deserializamos el contenido JSON en una lista de objetos 'Cadete'.
            // Si por algún motivo el proceso falla, devolvemos una lista vacía como respaldo.
            return JsonSerializer.Deserialize<List<Cadete>>(json, opciones) ?? new List<Cadete>();
        }
    }
}
