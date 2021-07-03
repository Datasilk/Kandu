namespace Kandu.Vendor
{
    /// <summary>
    /// A set of special events raised by Saber, for example, when a user account is created or permanently deleted.
    /// </summary>
    public abstract class KanduEvents
    {
        /// <summary>
        /// An event raised whenever a user is created
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        public void CreatedUser(int userId, string name, string email) { }
        /// <summary>
        /// An event raised whenever a user account & all associated data is permanently deleted from the database
        /// </summary>
        /// <param name="userId"></param>
        public void PermDeletedUser(int userId) { }

    }
}
