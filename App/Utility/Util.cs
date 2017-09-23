using System;

namespace Kandu.Utility
{
    public class Util
    {
        public Str Str;
        public Serializer Serializer;
        public Random Random = new Random();

        public Util()
        {
            Str = new Str(this);
            Serializer = new Serializer(this);
        }

        #region "Validation"

        public bool IsEmpty(object obj)
        {
            if(obj == null) { return true; }
            if (string.IsNullOrEmpty(obj.ToString())==true) { return true; }
            return false;
        }

        #endregion
        
    }
}
