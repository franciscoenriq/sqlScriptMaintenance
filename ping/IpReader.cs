using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace ping
{
    internal class IpReader
    {
        public static string[] ReadIpFromExcel(string filepath)
        {
            // Asegúrate de que ExcelPackage está configurado para un uso no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Verifica si el archivo existe antes de intentar abrirlo
            if (!File.Exists(filepath))
            {
                Console.WriteLine("El archivo no existe en la ruta especificada.");
                return new string[0]; // Retorna un array vacío si el archivo no existe
            }

            var ips = new List<string>();
            try
            {
                using (var package = new ExcelPackage(new FileInfo(filepath)))
                {
                    // Verifica que la hoja exista
                    var worksheet = package.Workbook.Worksheets["Hoja1"];
                    if (worksheet == null)
                    {
                        Console.WriteLine("La hoja 'Hoja1' no existe.");
                        return new string[0]; // Retorna un array vacío si la hoja no existe
                    }

                    var rowCount = worksheet.Dimension.Rows;

                    for (int i = 1; i <= rowCount; i++) // Comienza en 1 para usar índices válidos
                    {
                        var ip = worksheet.Cells[i, 4].Text; // Se extraen las IPs de la columna 4
                        ips.Add(ip);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el archivo Excel: " + ex.Message);
                return new string[0]; // Retorna un array vacío en caso de error
            }

            return ips.ToArray(); // Devuelve las direcciones IP como un array
        }
    }
}
