using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace GetNovelsApp.Core.Conexiones.DB
{
    internal class DataBaseAccess
    {
        private static string GetConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        internal static IDbConnection GetConnection()
        {
            return new SQLiteConnection(GetConnectionString(), true);
        }
    }
}
