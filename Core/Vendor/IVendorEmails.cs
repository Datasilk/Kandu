namespace Kandu.Vendor
{
    /// <summary>
    /// Define a list of unique email messages that your vendor plugin will be sending out to Kandu users
    /// </summary>
    public interface IVendorEmails
    {
        EmailAction[] Types { get; set; }
    }
}


