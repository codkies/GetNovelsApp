using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace GetNovelsApp.Core.Conexiones.DB
{
    internal class DataBaseAccess
    {
        static SQLiteConnection LastConecction;

        private static string GetConnectionString(string id = "New")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        internal static IDbConnection GetConnection()
        {
            LastConecction =  new SQLiteConnection(GetConnectionString(), true);
            return LastConecction;
        }

        /// <summary>
        /// Ultimo ID insertado. Cuidado de en el momento adecuado.
        /// </summary>
        internal static long LastID => LastConecction.LastInsertRowId;
    }
}
