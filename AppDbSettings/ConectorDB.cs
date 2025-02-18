using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace AppDbSettings
{
    public class ConectorDB
    {
        private string servidorSql;
        private string usuarioSql;
        private string contrasenaSql;
        private string cadenaConexion;
        public ConectorDB()
        {
            servidorSql = AppDbSettings.SettingsApp.Default.servidorSql;
            usuarioSql = AppDbSettings.SettingsApp.Default.usuarioSql;
            contrasenaSql = AppDbSettings.SettingsApp.Default.contrasenaSql;
            cadenaConexion = $"Server={servidorSql};User Id={usuarioSql};Password={contrasenaSql};TrustServerCertificate=True;";
        }


        public List<string> ConectarYConsultar()
        {
            List<string> listaBD = new List<string>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    string query = "SELECT name FROM sys.databases where database_id > 4; ";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        // Ejecutar la consulta y leer los resultados
                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            // Leer los resultados y agregarlos a la lista
                            while (reader.Read())
                            {
                                listaBD.Add(reader.GetString(0)); // Asume que 'name' es una columna de tipo string
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return listaBD;
        }
    }
}
