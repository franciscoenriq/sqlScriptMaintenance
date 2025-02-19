using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace EtlReporteria.DataAccess
{
    public class SqlServerConnection
    {
        public SqlConnection Connection { get; private set; }

        public SqlServerConnection(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }
}

