using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat_A_KafeBar
{
    internal class Connection
    {
        public static readonly string connectionString = ConfigurationManager.ConnectionStrings["bazahci"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
