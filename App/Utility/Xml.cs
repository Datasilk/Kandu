using System;
using System.Xml;

namespace Kandu.Utility
{
    public class Xml
    {

        public string GetAttribute(string name, XmlNode myNode)
        {
            if (!string.IsNullOrEmpty(myNode.Attributes[name].ToString())){
                return myNode.Attributes[name].Value;
            }
            return "";
        }

        public void SetAttribute(string name, string value, XmlNode myNode, XmlDocument myDoc)
        {
            if (myNode.Attributes.GetNamedItem(name) == null)
            {
                XmlAttribute newAttr = myDoc.CreateAttribute(name);
                newAttr.Value = value;
                myNode.Attributes.Append(newAttr);
            }
            else
            {
                myNode.Attributes[name].Value = value;
            }
        }

    }
}
