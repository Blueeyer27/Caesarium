using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesariumServer
{
    public static class CaesariumDatabase
    {
        private static string connStr;

        public static bool Create()
        {
            var defSettings = Properties.Settings.Default;

            connStr = "server=" + defSettings.db_host 
                + ";user=" + defSettings.db_user + ";port=" + defSettings.db_port 
                + ";password=" + defSettings.db_password + ";";

            ExecuteQuery("CREATE DATABASE IF NOT EXISTS `caesarium_db`;");

            connStr += "database=caesarium_db;";
            CreateTables();
            return true;
        }

        public static bool CreateTables()
        {
            ExecuteQuery(@"CREATE TABLE IF NOT EXISTS `accounts` (
             `ID` int(11) NOT NULL AUTO_INCREMENT,
             `Login` char(32) NOT NULL,
             `Password` char(32) NOT NULL,
             PRIMARY KEY (`ID`)) 
             ENGINE=InnoDB AUTO_INCREMENT=104 DEFAULT CHARSET=utf8;");
            return true;
        }

        public static void ExecuteQuery(string query)
        {
            try
            {
                using (var conn = new MySqlConnection(connStr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception on query execution: \n{0}", ex.Message);
            }
        }
    }
}
