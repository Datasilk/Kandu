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

                [XmlAttribute("scope")]
                public int Scope { get; set; }

                [XmlAttribute("scopeid")]
                public int ScopeId { get; set; }
            }
        }

        [Serializable]
        [XmlRoot("invites")]
        public class Invites
        {
            [XmlElement("invite")]
            public Invite[] List { get; set; }

            public class Invite
            {
                [XmlAttribute("id")]
                public int UserId { get; set; }

                [XmlAttribute("email")]
                public string Email { get; set; }

                [XmlAttribute("publickey")]
                public string PublicKey { get; set; }
            }
        }
    }
}
