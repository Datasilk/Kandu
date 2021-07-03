using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;

namespace Utility.Strings
{
    public static class Generic
    {

        #region "Conversion"
        public static int Asc(this string character)
        {
            string c = character.ToString();
            if (character.Length > 1) { c = c.Substring(0, 1); }

            return Encoding.ASCII.GetBytes(character)[0];
        }

        public static int Asc(this char character)
        {
            return Encoding.ASCII.GetBytes(new char[] { character })[0];
        }

        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[] GetBytes(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return string.Join("", chars);
        }

        public static string ReadToEnd(this Stream stream)
        {
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
        #endregion

        #region "Manipulation"
        public static string Right(this string str, int len)
        {
            return str.Substring(str.Length - len);
        }

        public static string Left(this string str, int len)
        {
            return str.Substring(0, len);
        }

        public static string ReplaceAll(this string myStr, string replaceWith, params string[] findList)
        {
            string newStr = myStr.ToString();
            for (int x = 0; x <= findList.Length - 1; x++)
            {
                newStr = newStr.Replace(findList[x], replaceWith.Replace("{1}", findList[x].Substring(0, 1)));
            }
            return newStr;
        }

        public static string ReplaceOnlyAlphaNumeric(this string myStr, bool allowAlpha = true, bool allowNumbers = true, bool allowSpaces = true)
        {
            string newStr = myStr.ToString();
            bool result = false;
            int x = 0;
            while (x < newStr.Length)
            {
                result = false;
                if (allowAlpha == true)
                {
                    if (Asc(newStr.Substring(x, 1)) >= Asc("a") & Asc(newStr.Substring(x, 1)) <= Asc("z"))
                    {
                        result = true;
                    }

                    if (Asc(newStr.Substring(x, 1)) >= Asc("A") & Asc(newStr.Substring(x, 1)) <= Asc("Z"))
                    {
                        result = true;
                    }
                }

                if (allowNumbers == true)
                {
                    if (Asc(newStr.Substring(x, 1)) >= Asc("0") & Asc(newStr.Substring(x, 1)) <= Asc("9"))
                    {
                        result = true;
                    }
                }

                if (allowSpaces == true)
                {
                    if (newStr.Substring(x, 1) == " ")
                    {
                        result = true;
                    }
                }

                if (result == false)
                {
                    //remove character
                    newStr = newStr.Substring(0, x - 1) + newStr.Substring(x + 1);
                }
                else
                {
                    x++;
                }
            }
            return newStr;
        }

        public static string Capitalize(this string origText)
        {
            string[] textParts = origText.Split(' ');
            for (int x = 0; x < textParts.Length; x++)
            {
                if (textParts[x] == "") { continue; }
                textParts[x] = textParts[x].Substring(0, 1).ToUpper() + textParts[x].Substring(1);
                if (textParts[x].Length > 2)
                {
                    if (textParts[x].Substring(0, 2) == "Mc" || textParts[x].Substring(0, 2) == "My")
                    {
                        textParts[x] = textParts[x].Substring(0, 2) + textParts[x].Substring(2, 1).ToUpper() + textParts[x].Substring(3);
                    }
                }
            }
            return string.Join(" ", textParts);
        }

        public static string MaxChars(this string str, int max, string trail = "")
        {
            if (str.Length > max)
            {
                return str.Substring(0, max) + trail;
            }
            return str;
        }

        #endregion

        #region "Generation"
        public static string NumberSuffix(int digit)
        {
            switch (digit)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    return "th";
                default:
                    switch (int.Parse(Right(digit.ToString(), 1)))
                    {
                        case 1:
                            return "st";
                        case 2:
                            return "nd";
                        case 3:
                            return "rd";
                    }
                    return "th";
            }
        }
        #endregion

        #region "Validation"
        public static bool IsNumeric(this string str)
        {
            double retNum;
            if (String.IsNullOrEmpty(str) == false)
            {
                return Double.TryParse(str, out retNum);
            }
            return false;
        }
        #endregion
    }

    public static class Generate
    {
        public static string NewId(int length = 3)
        {
            string result = "";
            for (var x = 0; x <= length - 1; x++)
            {
                int type = new Random().Next(1, 3);
                int num = 0;
                switch (type)
                {
                    case 1: //a-z
                        num = new Random().Next(0, 26);
                        result += (char)('a' + num);
                        break;

                    case 2: //A-Z
                        num = new Random().Next(0, 26);
                        result += (char)('A' + num);
                        break;

                    case 3: //0-9
                        num = new Random().Next(0, 9);
                        result += (char)('1' + num);
                        break;

                }

            }
            return result;
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            byte[] hash = md5.ComputeHash(bytes);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                str.Append(hash[i].ToString("X2"));
            }
            return str.ToString();
        }
    }

    public static class Validation
    {

        public static bool OnlyAlphabet(this string myStr, params string[] exceptionList)
        {
            bool result = false;
            for (int x = 0; x <= myStr.Length - 1; x++)
            {
                result = false;
                if (Generic.Asc(myStr.Substring(x, 1)) >= Generic.Asc("a") & Generic.Asc(myStr.Substring(x, 1)) <= Generic.Asc("z"))
                {
                    result = true;
                }
                if (Generic.Asc(myStr.Substring(x, 1)) >= Generic.Asc("A") & Generic.Asc(myStr.Substring(x, 1)) <= Generic.Asc("Z"))
                {
                    result = true;
                }
                if (exceptionList.Length >= 0)
                {
                    for (int y = exceptionList.GetLowerBound(0); y <= exceptionList.GetUpperBound(0); y++)
                    {
                        if (myStr.Substring(x, 1) == exceptionList[y])
                        {
                            result = true;
                        }
                    }
                }
                if (result == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool OnlyLettersAndNumbers(this string myStr, params string[] exceptionList)
        {
            bool result = false;
            for (int x = 0; x <= myStr.Length - 1; x++)
            {
                result = false;
                if (Generic.Asc(myStr.Substring(x, 1)) >= Generic.Asc("a") & Generic.Asc(myStr.Substring(x, 1)) <= Generic.Asc("z"))
                {
                    result = true;
                }

                if (Generic.Asc(myStr.Substring(x, 1)) >= Generic.Asc("A") & Generic.Asc(myStr.Substring(x, 1)) <= Generic.Asc("Z"))
                {
                    result = true;
                }

                if (Generic.Asc(myStr.Substring(x, 1)) >= Generic.Asc("0") & Generic.Asc(myStr.Substring(x, 1)) <= Generic.Asc("9"))
                {
                    result = true;
                }

                if (exceptionList.Length >= 0)
                {
                    for (int y = exceptionList.GetLowerBound(0); y <= exceptionList.GetUpperBound(0); y++)
                    {
                        if (myStr.Substring(x, 1) == exceptionList[y])
                        {
                            result = true;
                        }
                    }
                }

                if (result == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckChar(this char character, bool allowAlpha = true, bool allowNumbers = true, char[] allowList = null, bool capitalizedOnly = false)
        {
            if (allowAlpha == true)
            {
                if (Generic.Asc(character) >= Generic.Asc("a") & Generic.Asc(character) <= Generic.Asc("z"))
                {
                    if (capitalizedOnly == false)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

                if (Generic.Asc(character) >= Generic.Asc("A") & Generic.Asc(character) <= Generic.Asc("Z"))
                {
                    return true;
                }
            }

            if (allowNumbers == true)
            {
                if (Generic.Asc(character) >= Generic.Asc("0") & Generic.Asc(character) <= Generic.Asc("9"))
                {
                    return true;
                }
            }

            if ((allowList != null))
            {
                foreach (char c in allowList)
                {
                    if (c == character)
                        return true;
                }
            }

            return false;
        }

        public static bool ContainsCurseWords(this string txt)
        {
            string[] myCurse = new string[13];
            myCurse[0] = "fuck";
            myCurse[1] = "fukc";
            myCurse[2] = "bitch";
            myCurse[3] = "cunt";
            myCurse[4] = "slut";
            myCurse[5] = "whore";
            myCurse[6] = "nigger";
            myCurse[7] = "niger";
            myCurse[8] = "shit";
            myCurse[9] = "cum";
            myCurse[10] = "cock";
            myCurse[11] = "pussy";
            myCurse[12] = "vagina";

            string newtxt = txt.ToLower();
            for (int x = 0; x <= myCurse.GetUpperBound(0); x++)
            {
                if (newtxt.IndexOf(myCurse[x]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class Dates
    {
        public static string DateSentence(DateTime myDate, string dateSeparator = "-")
        {
            TimeSpan timespan = (DateTime.Now - myDate);
            if (timespan.Seconds < 30)
            {
                return "Moments ago";
            }
            else if (timespan.Seconds < 60)
            {
                return "About a minute ago";
            }
            else if (timespan.Minutes < 55)
            {
                return timespan.Minutes + " minutes ago";
            }
            else if (timespan.Hours < 1)
            {
                return "About an hour ago";
            }
            else if (timespan.Hours < 24)
            {
                return timespan.Hours + " hours ago";
            }
            else if (timespan.Days == 1)
            {
                return "Yesterday at " + string.Format("{0:t}", myDate);
            }
            else if (timespan.Days > 1 & timespan.Days < 30)
            {
                return timespan.Days + " days ago at " + string.Format("{0:t}", myDate);
            }
            else if (timespan.Days >= 30)
            {
                return "On " + myDate.ToString("M" + dateSeparator + "dd" + dateSeparator + "yyyy") + " at " + string.Format("{0:t}", myDate);
            }
            return "";
        }
    }

    public static class FileSystem
    {
        public static string GetFilename(this string filepath)
        {
            var paths = new string[] { };
            if (filepath.IndexOf("\\") >= 0)
            {
                paths = filepath.Split('\\');
            }
            else
            {
                paths = filepath.Split('/');
            }

            if (paths.Length > 1)
            {
                return paths[paths.Length - 1];
            }
            return filepath;
        }

        public static string GetFileExtension(this string filename)
        {
            for (int x = filename.Length - 1; x >= 0; x += -1)
            {
                if (filename.Substring(x, 1) == ".")
                {
                    return filename.Substring(x + 1);
                }
            }

            return "";
        }

        public static string GetFolder(this string filename)
        {
            var paths = filename.Replace("/", "\\").Split('\\').ToList();
            if (paths[paths.Count - 1].IndexOf(".") > 0)
            {
                paths.RemoveAt(paths.Count - 1);
            }
            return string.Join("\\", paths.ToArray()) + "\\";
        }

        public static string DateFolders(DateTime myDate, int folderCount = 3)
        {
            //generate folder paths based on given date
            string myMonth = myDate.Month.ToString();
            if (myMonth.Length == 1)
                myMonth = "0" + myMonth;
            string myDay = myDate.Day.ToString();
            if (myDay.Length == 1)
                myDay = "0" + myDay;
            if (folderCount == 3)
            {
                return myDate.Year.ToString() + myMonth + "/" + myDay;
            }
            else if (folderCount == 1)
            {
                return myDate.Year.ToString() + myMonth;
            }
            else if (folderCount == 2)
            {
                return myDay;
            }
            return "";
        }
    }

    public static class Web
    {
        public static string CleanUrl(this string url, bool queryString = true, bool hash = false, string[] removeFromQuery = null)
        {
            var u = url.Split(new char[] { '?' }, 2);
            var result = u[0];
            if (queryString == true && u.Length > 1)
            {
                if (hash == false && u[1].IndexOf("#") >= 0)
                {
                    result += "?" + u[1].Split('#')[0];
                }
                else
                {
                    result += "?" + u[1];
                }
            }
            else
            {
                if (hash == false && u[0].IndexOf("#") >= 0)
                {
                    result = u[0].Split('#')[0];
                }
            }
            if (removeFromQuery != null)
            {
                //remove specific query keys from url
                u = result.Split(new char[] { '?' }, 2);
                if (u.Length == 2)
                {
                    //get key values
                    var kv = u[1].ToLower().Split('&');
                    var k2v = new string[] { };
                    var newkeys = "";
                    foreach (var k in kv)
                    {
                        if (k != "")
                        {
                            if (k.IndexOf('=') > 0)
                            {
                                k2v = k.Split(new char[] { '=' }, 2);
                                if (!removeFromQuery.Contains(k2v[0]))
                                {
                                    newkeys += (newkeys.Length > 0 ? "&" : "") + k2v[0] + "=" + k2v[1];
                                }
                            }
                        }

                    }
                    if (newkeys != "") { newkeys = "?" + newkeys; }
                    result = u[0] + newkeys;
                }
            }

            return result;
        }

        public static string RemoveHtml(this string str, bool includeBR = false)
        {
            string RegExStr = "<[^>]*>";
            if (includeBR == true)
                RegExStr = "(\\<)(?!br(\\s|\\/|\\>))(.*?\\>)";
            Regex S = new Regex(RegExStr);
            return S.Replace(str, "");
        }

        public static string MinifyJS(this string js)
        {
            string result = js;
            //trim left
            result = Regex.Replace(result, "^\\s*", String.Empty, RegexOptions.ECMAScript);
            //trim right
            result = Regex.Replace(result, "\\s*[\\r\\n]", "\n", RegexOptions.ECMAScript);
            //remove whitespace beside of left curly braced
            result = Regex.Replace(result, "\\s*{\\s*", "{", RegexOptions.ECMAScript);
            //remove whitespace beside of coma
            result = Regex.Replace(result, "\\s*,\\s*", ",", RegexOptions.ECMAScript);
            //remove whitespace beside of semicolon
            result = Regex.Replace(result, "\\s*;\\s*", ";", RegexOptions.ECMAScript);
            //remove newline after keywords
            result = Regex.Replace(result, "\\r\\n(?<=\\b(abstract|boolean|break|byte|case|catch|char|class|const|continue|default|delete|do|double|else|extends|false|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|long|native|new|null|package|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient|true|try|typeof|var|void|while|with|\\r\\n|\\})\\r\\n)", " ", RegexOptions.ECMAScript);

            //remove all newlines
            result = Regex.Replace(result, "\r\n", " ", RegexOptions.ECMAScript);
            return result;
        }

        public static string GetDomainName(this string url)
        {
            string[] tmpDomain = GetSubDomainAndDomain(url).Split(new char[] { '.' }, 3);
            if (tmpDomain.Length == 2)
            {
                return string.Join(".", tmpDomain);
            }
            else if (tmpDomain.Length == 3)
            {
                if (tmpDomain[1] == "co")
                {
                    return string.Join(".", tmpDomain); ;
                }
                return tmpDomain[1] + "." + tmpDomain[2];
            }
            return string.Join(".", tmpDomain);
        }

        public static string GetSubDomainAndDomain(this string url)
        {
            string strDomain = url.Replace("http://", "").Replace("https://", "").Replace("www.", "").Split('/')[0];
            if (strDomain.IndexOf("localhost") >= 0 | strDomain.IndexOf("192.168") >= 0)
            {
                strDomain = "datasilk.io";
            }
            return strDomain.Replace("/", "");
        }

        public static string[] GetDomainParts(this string url)
        {
            string domainparts = GetSubDomainAndDomain(url);
            string domain = GetDomainName(domainparts);
            string sub = domainparts.Replace(domain, "");
            if (sub != "")
            {
                return new string[] { sub, domain };
            }
            return new string[] { "", domain };
        }

    }
}
