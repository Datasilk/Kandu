using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kandu.Utility
{
    public class Security
    {
        private Core S;

        public Security(Core KanduCore)
        {
            S = KanduCore;
        }

        public bool isMalicious(string input, int maxLength, string safeRegex, string badRegex, string[] safeInputs, string[] badInputs, double safeThreshold = 0.5, double badThreshold = 0.1)
        {
            int safeFound = 0;
            int badFound = 0;

            //check max length
            if (input.Length > maxLength) { return true; }

            //count up total safe inputs & bad inputs found
            foreach (string x in safeInputs)
            {
                if(input.IndexOf(x) >= 0) { safeFound++; }
            }
            foreach (string x in badInputs)
            {
                if (input.IndexOf(x) >= 0) { badFound++; }
            }

            //check threshold of safe & bad inputs
            if(safeInputs.Length > 0)
            {
                if (safeInputs.Length / safeFound < safeThreshold)
                {
                    return true;
                }
            }
            if(badInputs.Length > 0)
            {
                if (badInputs.Length / badFound > badThreshold)
                {
                    return true;
                }
            }

            //check safe & bad regular expressions
            if(safeRegex.Length > 0)
            {
                Regex safeReg = new Regex(safeRegex);
                if (safeReg.IsMatch(input) == false)
                {
                    return true;
                }
            }

            if (badRegex.Length > 0)
            {
                Regex badReg = new Regex(badRegex);
                if (badReg.IsMatch(input) == true)
                {
                    return true;
                }
            }


            return false;
        }
    }
}
