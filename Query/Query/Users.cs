using System.Collections.Generic;

namespace Kandu.Query
{
    public class Users : QuerySql
    {
        public Users(string connectionString) : base(connectionString)
        {
        }

        public int CreateUser(Models.User user)
        {
            return Sql.ExecuteScalar<int>(
                "User_Create",
                new Dictionary<string, object>()
                {
                    {"name", user.name },
                    {"email", user.email },
                    {"password", user.password },
                    {"photo", user.photo }
                }
            );
        }

        public Models.User AuthenticateUser(string email, string password)
        {
            var list = Sql.Populate<Models.User>("User_Authenticate",
                new Dictionary<string, object>()
                {
                    {"email", email },
                    {"password", password }
                }
            );
            if (list.Count > 0) { return list[0]; }
            return null;
        }

        public void UpdatePassword(int userId, string password)
        {
            Sql.ExecuteNonQuery("User_UpdatePassword",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"password", password }
                }
            );
        }

        public string GetEmail(int userId)
        {
            return Sql.ExecuteScalar<string>("User_GetEmail",
                new Dictionary<string, object>()
                {
                    {"userId", userId }
                }
            );
        }

        public string GetPassword(string email)
        {
            return Sql.ExecuteScalar<string>("User_GetPassword",
                new Dictionary<string, object>()
                {
                    {"email", email }
                }
            );
        }

        public void UpdateEmail(int userId, string email)
        {
            Sql.ExecuteNonQuery("User_UpdateEmail",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"email", email }
                }
            );
        }

        public bool HasPasswords()
        {
            return Sql.ExecuteScalar<int>("Users_HasPasswords") == 1;
        }

        public bool HasAdmin()
        {
            return Sql.ExecuteScalar<int>("Users_HasAdmin") == 1;
        }

        public void KeepMenuOpen(int userId, bool keepOpen)
        {
            Sql.ExecuteNonQuery("User_KeepMenuOpen",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"keepmenu", keepOpen }
                }
            );
        }

        public void AllColor(int userId, bool allColor)
        {
            Sql.ExecuteNonQuery("User_AllColor",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"allcolor", allColor }
                }
            );
        }

        public Models.User GetInfo(int userId)
        {
            var list = Sql.Populate<Models.User>("User_GetInfo",
                new Dictionary<string, object>()
                {
                    {"userId", userId }
                }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }
    }
}
