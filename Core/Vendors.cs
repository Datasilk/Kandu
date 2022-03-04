using System;
using System.Collections.Generic;
using Kandu.Vendor;

namespace Kandu.Core
{
    public static class Vendors
    {
        public static List<VendorInfo> Details { get; set; } = new List<VendorInfo>();
        public static Dictionary<string, Type> Controllers { get; set; } = new Dictionary<string, Type>();
        public static Dictionary<string, Type> Services { get; set; } = new Dictionary<string, Type>();
        public static Dictionary<string, Type> Startups { get; set; } = new Dictionary<string, Type>();
        public static List<IVendorKeys> Keys { get; set; } = new List<IVendorKeys>();
        public static Dictionary<string, IVendorEmailClient> EmailClients { get; set; } = new Dictionary<string, IVendorEmailClient>();
        public static Dictionary<string, EmailAction> EmailTypes { get; set; } = new Dictionary<string, EmailAction>();
        public static List<KanduEvents> EventHandlers { get; set; } = new List<KanduEvents>();

        //Partial Views
        public static List<IVendorPartialView> PartialViewsUnsorted { get; set; } = new List<IVendorPartialView>();
        public static Dictionary<PartialViewKeys, List<IVendorPartialView>> PartialViews { get; set; } = new Dictionary<PartialViewKeys, List<IVendorPartialView>>();

        //Cards
        public static Dictionary<string, IVendorCard> Cards = new Dictionary<string, IVendorCard>();
    }

    public class VendorInfo : IVendorInfo
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public Vendor.Version Version { get; set; }
        public string DLL { get; set; }
        public string Assembly { get; set; }
        public string Path { get; set; }
        public Type Type { get; set; }
        public Dictionary<string, Type> Controllers { get; set; } = new Dictionary<string, Type>();
        public Dictionary<string, Type> Services { get; set; } = new Dictionary<string, Type>();
        public Dictionary<string, Type> Startups { get; set; } = new Dictionary<string, Type>();
        public List<IVendorKeys> Keys { get; set; } = new List<IVendorKeys>();
        public Dictionary<string, IVendorEmailClient> EmailClients { get; set; } = new Dictionary<string, IVendorEmailClient>();
        public Dictionary<string, EmailAction> EmailTypes { get; set; } = new Dictionary<string, EmailAction>();
    }

}
