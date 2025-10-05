using System.Text.Json;
using MiCadeteria.Models;

namespace MiCadeteria.AccesoADatos
{
    public class AccesoADatosCadeteria
    {
        // 🔒 Ruta del archivo JSON donde se guardan los datos generales de la cadetería.
        // Se define como readonly porque no se va a cambiar en tiempo de ejecución.
        private readonly string rutaArchivo = "Data/Cadeteria.json";

        // ✅ Método principal: obtiene la información de la cadetería desde el archivo JSON
        public Cadeteria Obtener()
        {
            // Si el archivo no existe, devolvemos una nueva Cadeteria vacía.
            // Esto evita que el sistema se rompa la primera vez que corre.
            if (!File.Exists(rutaArchivo))
            {
                return new Cadeteria
                {
                    Nombre = "Cadetería Sin Datos",
                    Telefono = "000-0000"
                };
            }

            // Leemos el contenido del archivo en formato texto
            string json = File.ReadAllText(rutaArchivo);

            // Si está vacío o mal escrito, devolvemos una Cadeteria genérica
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Cadeteria
                {
                    Nombre = "Cadetería Sin Datos",
                    Telefono = "000-0000"
                };
            }

            // Configuración del deserializador JSON:
            // - PropertyNameCaseInsensitive = true  → ignora mayúsculas/minúsculas en los nombres de propiedades
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Intentamos convertir el texto JSON a un objeto Cadeteria
            // Si algo falla, devolvemos una instancia vacía como respaldo.
            return JsonSerializer.Deserialize<Cadeteria>(json, opciones) ?? new Cadeteria
            {
                Nombre = "Cadetería Sin Datos",
                Telefono = "000-0000"
            };
        }
    }
}
