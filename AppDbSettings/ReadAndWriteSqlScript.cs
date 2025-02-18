using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppDbSettings
{
    internal class ReadAndWriteSqlScript
    {
        public static string LeerArchivoSql(string rutaArchivo)
        {
            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    throw new FileNotFoundException("El archivo SQL no se encuentra en la ruta especificada.");
                }
                // Leemos el contenido del archivo SQL
                string contenido = File.ReadAllText(rutaArchivo);
                return contenido;

            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al leer el archivo SQL: {ex.Message}");
                return null;
            }

        }

        public static void EscribirBasesDeDatos(string rutaArchivo, List<string> bases)
        {
            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    throw new FileNotFoundException("El archivo SQL no se encuentra en la ruta especificada.");
                }
                //leemos el archivo entero. 
                string contenido = File.ReadAllText(rutaArchivo);

                //definimos el primer bloque de codigo
                int posInsert = contenido.IndexOf("INSERT INTO @databases VALUES", StringComparison.OrdinalIgnoreCase);
                if (posInsert == -1)
                {
                    throw new InvalidOperationException("No se encontró la línea 'INSERT INTO @databases VALUES' en el archivo SQL.");
                }

                int posIteracion = contenido.IndexOf("-- Iteramos sobre las bases de datos que queremos mantener", posInsert, StringComparison.OrdinalIgnoreCase);
                if (posIteracion == -1)
                {
                    throw new InvalidOperationException("No se encontró el marcador de iteración en el archivo SQL.");
                }
                // aca definimos los bloques que vamos a concatenar. 
                string inicioArchivo = contenido.Substring(0, posInsert); // Todo lo anterior al INSERT
                string finArchivo = contenido.Substring(posIteracion);   // Todo lo posterior al comentario de iteración


                string basesDeDatosSql = string.Join(",\n ", bases.Select(bd => $"(N'{bd}')"));

                // Crear el nuevo bloque INSERT con el formato correcto
                string nuevoInsert = $"INSERT INTO @databases VALUES \n {basesDeDatosSql};\n\n";

                //  Concatenar todo el contenido en orden correcto
                string nuevoContenido = inicioArchivo + nuevoInsert + finArchivo;

                File.WriteAllText(rutaArchivo, nuevoContenido);

                Console.WriteLine("Archivo SQL actualizado correctamente con las bases de datos seleccionadas.");

            }

            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al escribir en el archivo SQL: {ex.Message}");
            }
        }

        public static List<string> ObtenerBDs(string rutaArchivo)
        {
            List<string> BDs = new List<string>();

            try
            {
                string contenido = LeerArchivoSql(rutaArchivo);
                if (string.IsNullOrEmpty(contenido)) return BDs;

                // Expresiones regulares para extraer la parte que nos interesa
                string patronInicio = @"INSERT INTO @databases VALUES";
                string patronFin = @"-- Iteramos sobre las bases de datos que queremos mantener";


                int inicio = contenido.IndexOf(patronInicio);
                int fin = contenido.IndexOf(patronFin);

                if (inicio == -1 || fin == -1 || fin <= inicio)
                {
                    Console.WriteLine("No se encontró la sección de bases de datos en el script.");
                    return BDs;
                }
                // Extraer solo la parte de bases de datos
                string bloqueBd = contenido.Substring(inicio + patronInicio.Length, fin - (inicio + patronInicio.Length));

                // Buscar todas las bases de datos dentro de los paréntesis
                MatchCollection matches = Regex.Matches(bloqueBd, @"\(N'([^']+)'\)");

                foreach (Match match in matches)
                {
                    BDs.Add(match.Groups[1].Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener bases de datos: {ex.Message}");
            }
            return BDs;
        }
    }
        
}

