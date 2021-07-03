using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace Query
{
    public static class Script
    {
        public static void Execute(string file)
        {
            string script = File.ReadAllText(file);
            SqlConnection conn = new SqlConnection(Sql.ConnectionString);
            Server server = new Server(new ServerConnection(conn));
            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}
