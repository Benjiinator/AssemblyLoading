using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PluginShared
{
    public class StorageManager
    {
        public readonly string _connectionString = string.Empty;

        public StorageManager(string database, string user, string password)
        {
            _connectionString = GetConnectionString(database, user, password);
        }

        private static string GetConnectionString(string dbname, string user = null, string password = null)
        {
            var conn = new SqlConnectionStringBuilder
            {
                ApplicationName = "DataRobotten StorageManager",
                ConnectRetryCount = 3,
                DataSource = "(localdb)\\MSSQLLocalDB",
                InitialCatalog = dbname
            };

            if (string.IsNullOrWhiteSpace(user))
            {
                conn.IntegratedSecurity = true;
            }
            else
            {
                conn.IntegratedSecurity = false;
                conn.Password = password;
                conn.UserID = user;
            }

            return conn.ConnectionString;
        }
    }
}
