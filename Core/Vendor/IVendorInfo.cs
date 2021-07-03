using System.Linq;

namespace Kandu.Vendor
{
    public interface IVendorInfo
    {
        /// <summary>
        /// The key used to identify the vendor plugin. This is used to find the namespace for your plugin (e.g. Kandu.Vendors.MyPlugin) 
        /// and is also used to find the folder path to the vendor plugin (e.g. /Vendors/MyPlugin/)
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// The human-readable name for your vendor plugin.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A short summary of what your plugin was design to do.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Optional. The relative path (/editor/vendors/my-plugin/icon.svg) to an SVG icon used within Kandu's Editor
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// The current version number of your plugin.
        /// </summary>
        Version Version { get; set; }
    }

    public class Version { 
        public int Major { get; set; }
        public int Minor1 { get; set; }
        public int Minor2 { get; set; }
        public int Minor3 { get; set; }
        public Version(int major, int minor1, int minor2, int minor3)
        {
            Major = major;
            Minor1 = minor1;
            Minor2 = minor2;
            Minor3= minor3;
        }

        public override string ToString()
        {
            return Major + "." + Minor1 + "." + Minor2 + "." + Minor3;
        }

        public static implicit operator string(Version version)
        {
            if(version == null) { return ""; }
            return version.Major + "." + version.Minor1 + "." + version.Minor2 + "." + version.Minor3;
        }

        public static implicit operator Version(string version)
        {
            var v = version.Split('.').Select(a => int.Parse(a)).ToArray();
            return new Version(v[0], v[1], v[2], v[3]);
        }
    }
}
