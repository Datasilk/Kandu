using System;
using System.Xml.Serialization;

namespace Query.Models
{
    public class Xml
    {
        [Serializable]
        [XmlRoot("keys")]
        public class Keys
        {
            [XmlElement("key")]
            public Key[] List { get; set; }

            public class Key
            {
                [XmlAttribute("name")]
                public string Name { get; set; }

                [XmlAttribute("value")]
                public int Value { get; set; }
            }
        }
    }
}
