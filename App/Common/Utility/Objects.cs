using System;

namespace Utility
{
    public static class Objects
    {
        public static bool IsEmpty(object obj)
        {
            if (obj == null) { return true; }
            if (obj.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(obj.ToString()) == true) { return true; }
            }
            return false;
        }
    }
}
