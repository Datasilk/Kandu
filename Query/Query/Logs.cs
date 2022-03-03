namespace Query
{
    public static class Logs
    {
        public static void LogError(int userId, string url, string area, string message, string stacktrace)
        {
            Sql.ExecuteNonQuery("Log_Error", new { userId, url, area, message, stacktrace });
        }

    }
}
