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
             ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=utf8;");
            return true;
        }

        public static List<String> ExecuteQuery(string query, params object[] param)
        {
            var result = new List<String>();

            try
            {
                using (var conn = new MySqlConnection(connStr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = query;
                    for (var i = 0; i < param.Length; i += 2)
                    {
                        cmd.Parameters.AddWithValue(param[i].ToString(), param[i + 1]);
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetString(i));
                            }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception on query execution: \n{0}", ex.Message);
            }

            return result;
        }
    }
}
