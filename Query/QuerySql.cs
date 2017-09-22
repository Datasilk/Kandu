
namespace Kandu.Query
{
    public class QuerySql
    {

        public Sql Sql;

        public QuerySql(string connectionString)
        {
            Sql = new Sql(connectionString);
        }

    }
}
