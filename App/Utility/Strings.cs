using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Kandu.Utility
{
    public class Str
    {
        private Util Util;

        public Str(Util util)
        {
            Util = util;
        }

        #region "Conversion"
        public int Asc(string character)
        {
            string c = character.ToString();
            if (character.Length > 1) { c = c.Substring(0, 1); }

            return Encoding.ASCII.GetBytes(character)[0];
        }

        public string FromBoolToIntString(bool value)
        {
            return (value == true ? "1" : "0");
        }

        public byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public byte[] GetBytes(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return string.Join("", chars);
        }

        public string ReadStream(Stream stream)
        {
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public double[] GetNumbers(string numberArray)
        {
            if (numberArray.Length == 0) { return new double[] { }; }
            var nums = new List<double>();
            var arr = numberArray.Replace(", ", ",").Replace(" ,", ",").Split(',');
            foreach (var a in arr)
            {
                if (IsNumeric(a))
                {
                    nums.Add(double.Parse(a));
                }
            }
            return nums.ToArray();
        }

        public int[] GetInts(string numberArray)
        {
            if (numberArray.Length == 0) { return new int[] { }; }
            var nums = new List<int>();
            var arr = numberArray.Replace(", ", ",").Replace(" ,", ",").Split(',');
            foreach (var a in arr)
            {
                if (IsNumeric(a))
                {
                    nums.Add(int.Parse(a));
                }
            }
            return nums.ToArray();
        }
        #endregion

        #region "Manipulation"
        public string Right(string str, int len)
        {
            return str.Substring(0, str.Length - 1 - len);
        }

        public string Left(string str, int len)
        {
            return str.Substring(0 + len);
        }

        public string replaceAll(string myStr, string replaceWith, params string[] findList)
        {
            string newStr = myStr.ToString();
            for (int x = 0; x <= findList.Length - 1; x++)
            {
                newStr = newStr.Replace(findList[x], replaceWith.Replace("{1}", findList[x].Substring(0, 1)));
            }
            return newStr;
        }

        public string replaceOnlyAlphaNumeric(string myStr, bool allowAlpha = true, bool allowNumbers = true, bool allowSpaces = true)
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

        public string Capitalize(string origText)
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

        public string CleanHtml(string html)
        {
            return html;
            //return Regex.Replace(html, "\\s{2,}", " ").Replace("> <", "><");
        }

        public string CleanUrl(string url, bool queryString = true, bool hash = false, string[] removeFromQuery = null)
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
            if (Util.IsEmpty(removeFromQuery) == false)
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

        public string UrlEncode(string text)
        {
            var chars = new string[] { " " };
            var encoded = new string[] { "%20" };
            var url = text.ToString();
            for (var x = 0; x < chars.Length; x++)
            {
                url = url.Replace(chars[x], encoded[x]);
            }

            return url;
        }

        public string UrlDecode(string text)
        {
            var chars = new string[] { " " };
            var encoded = new string[] { "%20" };
            var url = text.ToString();
            for (var x = 0; x < chars.Length; x++)
            {
                url = url.Replace(encoded[x], chars[x]);
            }

            return url;
        }

        public string HtmlEncode(string text)
        {
            var html1 = new string[] { "&nbsp;", "&iexcl;", "&cent;", "&pound;", "&curren;", "&yen;", "&brvbar;", "&sect;", "&uml;", "&copy;", "&ordf;", "&laquo;", "&not;", "&shy;", "&reg;", "&macr;", "&deg;", "&plusmn;", "&sup2;", "&sup3;", "&acute;", "&micro;", "&para;", "&middot;", "&cedil;", "&sup1;", "&ordm;", "&raquo;", "&frac14;", "&frac12;", "&frac34;", "&iquest;", "&Agrave;", "&Aacute;", "&Acirc;", "&Atilde;", "&Auml;", "&Aring;", "&AElig;", "&Ccedil;", "&Egrave;", "&Eacute;", "&Ecirc;", "&Euml;", "&Igrave;", "&Iacute;", "&Icirc;", "&Iuml;", "&ETH;", "&Ntilde;", "&Ograve;", "&Oacute;", "&Ocirc;", "&Otilde;", "&Ouml;", "&times;", "&Oslash;", "&Ugrave;", "&Uacute;", "&Ucirc;", "&Uuml;", "&Yacute;", "&THORN;", "&szlig;", "&agrave;", "&aacute;", "&acirc;", "&atilde;", "&auml;", "&aring;", "&aelig;", "&ccedil;", "&egrave;", "&eacute;", "&ecirc;", "&euml;", "&igrave;", "&iacute;", "&icirc;", "&iuml;", "&eth;", "&ntilde;", "&ograve;", "&oacute;", "&ocirc;", "&otilde;", "&ouml;", "&divide;", "&oslash;", "&ugrave;", "&uacute;", "&ucirc;", "&uuml;", "&yacute;", "&thorn;", "&yuml;", "&quot;", "&amp;", "&lt;", "&gt;", "&OElig;", "&oelig;", "&Scaron;", "&scaron;", "&Yuml;", "&circ;", "&tilde;", "&ensp;", "&emsp;", "&thinsp;", "&zwnj;", "&zwj;", "&lrm;", "&rlm;", "&ndash;", "&mdash;", "&lsquo;", "&rsquo;", "&sbquo;", "&ldquo;", "&rdquo;", "&bdquo;", "&dagger;", "&Dagger;", "&permil;", "&lsaquo;", "&rsaquo;", "&euro;", "&fnof;", "&Alpha;", "&Beta;", "&Gamma;", "&Delta;", "&Epsilon;", "&Zeta;", "&Eta;", "&Theta;", "&Iota;", "&Kappa;", "&Lambda;", "&Mu;", "&Nu;", "&Xi;", "&Omicron;", "&Pi;", "&Rho;", "&Sigma;", "&Tau;", "&Upsilon;", "&Phi;", "&Chi;", "&Psi;", "&Omega;", "&alpha;", "&beta;", "&gamma;", "&delta;", "&epsilon;", "&zeta;", "&eta;", "&theta;", "&iota;", "&kappa;", "&lambda;", "&mu;", "&nu;", "&xi;", "&omicron;", "&pi;", "&rho;", "&sigmaf;", "&sigma;", "&tau;", "&upsilon;", "&phi;", "&chi;", "&psi;", "&omega;", "&thetasym;", "&upsih;", "&piv;", "&bull;", "&hellip;", "&prime;", "&Prime;", "&oline;", "&frasl;", "&weierp;", "&image;", "&real;", "&trade;", "&alefsym;", "&larr;", "&uarr;", "&rarr;", "&darr;", "&harr;", "&crarr;", "&lArr;", "&uArr;", "&rArr;", "&dArr;", "&hArr;", "&forall;", "&part;", "&exist;", "&empty;", "&nabla;", "&isin;", "&notin;", "&ni;", "&prod;", "&sum;", "&minus;", "&lowast;", "&radic;", "&prop;", "&infin;", "&ang;", "&and;", "&or;", "&cap;", "&cup;", "&int;", "&there4;", "&sim;", "&cong;", "&asymp;", "&ne;", "&equiv;", "&le;", "&ge;", "&sub;", "&sup;", "&nsub;", "&sube;", "&supe;", "&oplus;", "&otimes;", "&perp;", "&sdot;", "&lceil;", "&rceil;", "&lfloor;", "&rfloor;", "&lang;", "&rang;", "&loz;", "&spades;", "&clubs;", "&hearts;", "&diams;" };
            var html2 = new string[] { "&#160;", "&#161;", "&#162;", "&#163;", "&#164;", "&#165;", "&#166;", "&#167;", "&#168;", "&#169;", "&#170;", "&#171;", "&#172;", "&#173;", "&#174;", "&#175;", "&#176;", "&#177;", "&#178;", "&#179;", "&#180;", "&#181;", "&#182;", "&#183;", "&#184;", "&#185;", "&#186;", "&#187;", "&#188;", "&#189;", "&#190;", "&#191;", "&#192;", "&#193;", "&#194;", "&#195;", "&#196;", "&#197;", "&#198;", "&#199;", "&#200;", "&#201;", "&#202;", "&#203;", "&#204;", "&#205;", "&#206;", "&#207;", "&#208;", "&#209;", "&#210;", "&#211;", "&#212;", "&#213;", "&#214;", "&#215;", "&#216;", "&#217;", "&#218;", "&#219;", "&#220;", "&#221;", "&#222;", "&#223;", "&#224;", "&#225;", "&#226;", "&#227;", "&#228;", "&#229;", "&#230;", "&#231;", "&#232;", "&#233;", "&#234;", "&#235;", "&#236;", "&#237;", "&#238;", "&#239;", "&#240;", "&#241;", "&#242;", "&#243;", "&#244;", "&#245;", "&#246;", "&#247;", "&#248;", "&#249;", "&#250;", "&#251;", "&#252;", "&#253;", "&#254;", "&#255;", "&#34;", "&#38;", "&#60;", "&#62;", "&#338;", "&#339;", "&#352;", "&#353;", "&#376;", "&#710;", "&#732;", "&#8194;", "&#8195;", "&#8201;", "&#8204;", "&#8205;", "&#8206;", "&#8207;", "&#8211;", "&#8212;", "&#8216;", "&#8217;", "&#8218;", "&#8220;", "&#8221;", "&#8222;", "&#8224;", "&#8225;", "&#8240;", "&#8249;", "&#8250;", "&#8364;", "&#402;", "&#913;", "&#914;", "&#915;", "&#916;", "&#917;", "&#918;", "&#919;", "&#920;", "&#921;", "&#922;", "&#923;", "&#924;", "&#925;", "&#926;", "&#927;", "&#928;", "&#929;", "&#931;", "&#932;", "&#933;", "&#934;", "&#935;", "&#936;", "&#937;", "&#945;", "&#946;", "&#947;", "&#948;", "&#949;", "&#950;", "&#951;", "&#952;", "&#953;", "&#954;", "&#955;", "&#956;", "&#957;", "&#958;", "&#959;", "&#960;", "&#961;", "&#962;", "&#963;", "&#964;", "&#965;", "&#966;", "&#967;", "&#968;", "&#969;", "&#977;", "&#978;", "&#982;", "&#8226;", "&#8230;", "&#8242;", "&#8243;", "&#8254;", "&#8260;", "&#8472;", "&#8465;", "&#8476;", "&#8482;", "&#8501;", "&#8592;", "&#8593;", "&#8594;", "&#8595;", "&#8596;", "&#8629;", "&#8656;", "&#8657;", "&#8658;", "&#8659;", "&#8660;", "&#8704;", "&#8706;", "&#8707;", "&#8709;", "&#8711;", "&#8712;", "&#8713;", "&#8715;", "&#8719;", "&#8721;", "&#8722;", "&#8727;", "&#8730;", "&#8733;", "&#8734;", "&#8736;", "&#8743;", "&#8744;", "&#8745;", "&#8746;", "&#8747;", "&#8756;", "&#8764;", "&#8773;", "&#8776;", "&#8800;", "&#8801;", "&#8804;", "&#8805;", "&#8834;", "&#8835;", "&#8836;", "&#8838;", "&#8839;", "&#8853;", "&#8855;", "&#8869;", "&#8901;", "&#8968;", "&#8969;", "&#8970;", "&#8971;", "&#9001;", "&#9002;", "&#9674;", "&#9824;", "&#9827;", "&#9829;", "&#9830;" };
            var chars = new int[] { 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 34, 38, 60, 62, 338, 339, 352, 353, 376, 710, 732, 8194, 8195, 8201, 8204, 8205, 8206, 8207, 8211, 8212, 8216, 8217, 8218, 8220, 8221, 8222, 8224, 8225, 8240, 8249, 8250, 8364, 402, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 977, 978, 982, 8226, 8230, 8242, 8243, 8254, 8260, 8472, 8465, 8476, 8482, 8501, 8592, 8593, 8594, 8595, 8596, 8629, 8656, 8657, 8658, 8659, 8660, 8704, 8706, 8707, 8709, 8711, 8712, 8713, 8715, 8719, 8721, 8722, 8727, 8730, 8733, 8734, 8736, 8743, 8744, 8745, 8746, 8747, 8756, 8764, 8773, 8776, 8800, 8801, 8804, 8805, 8834, 8835, 8836, 8838, 8839, 8853, 8855, 8869, 8901, 8968, 8969, 8970, 8971, 9001, 9002, 9674, 9824, 9827, 9829, 9830 };
            var decoded = new string[] { " ", "¡", "¢", "£", "¤", "¥", "¦", "§", "¨", "©", "ª", "«", "¬", "­", "®", "¯", "°", "±", "²", "³", "´", "µ", "¶", "·", "¸", "¹", "º", "»", "¼", "½", "¾", "¿", "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Ð", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "×", "Ø", "Ù", "Ú", "Û", "Ü", "Ý", "Þ", "ß", "à", "á", "â", "ã", "ä", "å", "æ", "ç", "è", "é", "ê", "ë", "ì", "í", "î", "ï", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "÷", "ø", "ù", "ú", "û", "ü", "ý", "þ", "ÿ", "\"", "&", "<", ">", "Œ", "œ", "Š", "š", "Ÿ", "ˆ", "˜", " ", " ", " ", "‌", "‍", "‎", "‏", "–", "—", "‘", "’", "‚", "“", "”", "„", "†", "‡", "‰", "‹", "›", "€", "ƒ", "Α", "Β", "Γ", "Δ", "Ε", "Ζ", "Η", "Θ", "Ι", "Κ", "Λ", "Μ", "Ν", "Ξ", "Ο", "Π", "Ρ", "Σ", "Τ", "Υ", "Φ", "Χ", "Ψ", "Ω", "α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι", "κ", "λ", "μ", "ν", "ξ", "ο", "π", "ρ", "ς", "σ", "τ", "υ", "φ", "χ", "ψ", "ω", "ϑ", "ϒ", "ϖ", "•", "…", "′", "″", "‾", "⁄", "℘", "ℑ", "ℜ", "™", "ℵ", "←", "↑", "→", "↓", "↔", "↵", "⇐", "⇑", "⇒", "⇓", "⇔", "∀", "∂", "∃", "∅", "∇", "∈", "∉", "∋", "∏", "∑", "−", "∗", "√", "∝", "∞", "∠", "∧", "∨", "∩", "∪", "∫", "∴", "∼", "≅", "≈", "≠", "≡", "≤", "≥", "⊂", "⊃", "⊄", "⊆", "⊇", "⊕", "⊗", "⊥", "⋅", "⌈", "⌉", "⌊", "⌋", "⟨", "⟩", "◊", "♠", "♣", "♥", "♦" };
            var htm = text.ToString();
            for (int x = 0; x < html1.Length; x++)
            {
                htm = htm.Replace(decoded[x], html1[x]);
            }
            return htm;
        }

        public string HtmlDecode(string html)
        {
            var html1 = new string[] { "&nbsp;", "&iexcl;", "&cent;", "&pound;", "&curren;", "&yen;", "&brvbar;", "&sect;", "&uml;", "&copy;", "&ordf;", "&laquo;", "&not;", "&shy;", "&reg;", "&macr;", "&deg;", "&plusmn;", "&sup2;", "&sup3;", "&acute;", "&micro;", "&para;", "&middot;", "&cedil;", "&sup1;", "&ordm;", "&raquo;", "&frac14;", "&frac12;", "&frac34;", "&iquest;", "&Agrave;", "&Aacute;", "&Acirc;", "&Atilde;", "&Auml;", "&Aring;", "&AElig;", "&Ccedil;", "&Egrave;", "&Eacute;", "&Ecirc;", "&Euml;", "&Igrave;", "&Iacute;", "&Icirc;", "&Iuml;", "&ETH;", "&Ntilde;", "&Ograve;", "&Oacute;", "&Ocirc;", "&Otilde;", "&Ouml;", "&times;", "&Oslash;", "&Ugrave;", "&Uacute;", "&Ucirc;", "&Uuml;", "&Yacute;", "&THORN;", "&szlig;", "&agrave;", "&aacute;", "&acirc;", "&atilde;", "&auml;", "&aring;", "&aelig;", "&ccedil;", "&egrave;", "&eacute;", "&ecirc;", "&euml;", "&igrave;", "&iacute;", "&icirc;", "&iuml;", "&eth;", "&ntilde;", "&ograve;", "&oacute;", "&ocirc;", "&otilde;", "&ouml;", "&divide;", "&oslash;", "&ugrave;", "&uacute;", "&ucirc;", "&uuml;", "&yacute;", "&thorn;", "&yuml;", "&quot;", "&amp;", "&lt;", "&gt;", "&OElig;", "&oelig;", "&Scaron;", "&scaron;", "&Yuml;", "&circ;", "&tilde;", "&ensp;", "&emsp;", "&thinsp;", "&zwnj;", "&zwj;", "&lrm;", "&rlm;", "&ndash;", "&mdash;", "&lsquo;", "&rsquo;", "&sbquo;", "&ldquo;", "&rdquo;", "&bdquo;", "&dagger;", "&Dagger;", "&permil;", "&lsaquo;", "&rsaquo;", "&euro;", "&fnof;", "&Alpha;", "&Beta;", "&Gamma;", "&Delta;", "&Epsilon;", "&Zeta;", "&Eta;", "&Theta;", "&Iota;", "&Kappa;", "&Lambda;", "&Mu;", "&Nu;", "&Xi;", "&Omicron;", "&Pi;", "&Rho;", "&Sigma;", "&Tau;", "&Upsilon;", "&Phi;", "&Chi;", "&Psi;", "&Omega;", "&alpha;", "&beta;", "&gamma;", "&delta;", "&epsilon;", "&zeta;", "&eta;", "&theta;", "&iota;", "&kappa;", "&lambda;", "&mu;", "&nu;", "&xi;", "&omicron;", "&pi;", "&rho;", "&sigmaf;", "&sigma;", "&tau;", "&upsilon;", "&phi;", "&chi;", "&psi;", "&omega;", "&thetasym;", "&upsih;", "&piv;", "&bull;", "&hellip;", "&prime;", "&Prime;", "&oline;", "&frasl;", "&weierp;", "&image;", "&real;", "&trade;", "&alefsym;", "&larr;", "&uarr;", "&rarr;", "&darr;", "&harr;", "&crarr;", "&lArr;", "&uArr;", "&rArr;", "&dArr;", "&hArr;", "&forall;", "&part;", "&exist;", "&empty;", "&nabla;", "&isin;", "&notin;", "&ni;", "&prod;", "&sum;", "&minus;", "&lowast;", "&radic;", "&prop;", "&infin;", "&ang;", "&and;", "&or;", "&cap;", "&cup;", "&int;", "&there4;", "&sim;", "&cong;", "&asymp;", "&ne;", "&equiv;", "&le;", "&ge;", "&sub;", "&sup;", "&nsub;", "&sube;", "&supe;", "&oplus;", "&otimes;", "&perp;", "&sdot;", "&lceil;", "&rceil;", "&lfloor;", "&rfloor;", "&lang;", "&rang;", "&loz;", "&spades;", "&clubs;", "&hearts;", "&diams;" };
            var html2 = new string[] { "&#160;", "&#161;", "&#162;", "&#163;", "&#164;", "&#165;", "&#166;", "&#167;", "&#168;", "&#169;", "&#170;", "&#171;", "&#172;", "&#173;", "&#174;", "&#175;", "&#176;", "&#177;", "&#178;", "&#179;", "&#180;", "&#181;", "&#182;", "&#183;", "&#184;", "&#185;", "&#186;", "&#187;", "&#188;", "&#189;", "&#190;", "&#191;", "&#192;", "&#193;", "&#194;", "&#195;", "&#196;", "&#197;", "&#198;", "&#199;", "&#200;", "&#201;", "&#202;", "&#203;", "&#204;", "&#205;", "&#206;", "&#207;", "&#208;", "&#209;", "&#210;", "&#211;", "&#212;", "&#213;", "&#214;", "&#215;", "&#216;", "&#217;", "&#218;", "&#219;", "&#220;", "&#221;", "&#222;", "&#223;", "&#224;", "&#225;", "&#226;", "&#227;", "&#228;", "&#229;", "&#230;", "&#231;", "&#232;", "&#233;", "&#234;", "&#235;", "&#236;", "&#237;", "&#238;", "&#239;", "&#240;", "&#241;", "&#242;", "&#243;", "&#244;", "&#245;", "&#246;", "&#247;", "&#248;", "&#249;", "&#250;", "&#251;", "&#252;", "&#253;", "&#254;", "&#255;", "&#34;", "&#38;", "&#60;", "&#62;", "&#338;", "&#339;", "&#352;", "&#353;", "&#376;", "&#710;", "&#732;", "&#8194;", "&#8195;", "&#8201;", "&#8204;", "&#8205;", "&#8206;", "&#8207;", "&#8211;", "&#8212;", "&#8216;", "&#8217;", "&#8218;", "&#8220;", "&#8221;", "&#8222;", "&#8224;", "&#8225;", "&#8240;", "&#8249;", "&#8250;", "&#8364;", "&#402;", "&#913;", "&#914;", "&#915;", "&#916;", "&#917;", "&#918;", "&#919;", "&#920;", "&#921;", "&#922;", "&#923;", "&#924;", "&#925;", "&#926;", "&#927;", "&#928;", "&#929;", "&#931;", "&#932;", "&#933;", "&#934;", "&#935;", "&#936;", "&#937;", "&#945;", "&#946;", "&#947;", "&#948;", "&#949;", "&#950;", "&#951;", "&#952;", "&#953;", "&#954;", "&#955;", "&#956;", "&#957;", "&#958;", "&#959;", "&#960;", "&#961;", "&#962;", "&#963;", "&#964;", "&#965;", "&#966;", "&#967;", "&#968;", "&#969;", "&#977;", "&#978;", "&#982;", "&#8226;", "&#8230;", "&#8242;", "&#8243;", "&#8254;", "&#8260;", "&#8472;", "&#8465;", "&#8476;", "&#8482;", "&#8501;", "&#8592;", "&#8593;", "&#8594;", "&#8595;", "&#8596;", "&#8629;", "&#8656;", "&#8657;", "&#8658;", "&#8659;", "&#8660;", "&#8704;", "&#8706;", "&#8707;", "&#8709;", "&#8711;", "&#8712;", "&#8713;", "&#8715;", "&#8719;", "&#8721;", "&#8722;", "&#8727;", "&#8730;", "&#8733;", "&#8734;", "&#8736;", "&#8743;", "&#8744;", "&#8745;", "&#8746;", "&#8747;", "&#8756;", "&#8764;", "&#8773;", "&#8776;", "&#8800;", "&#8801;", "&#8804;", "&#8805;", "&#8834;", "&#8835;", "&#8836;", "&#8838;", "&#8839;", "&#8853;", "&#8855;", "&#8869;", "&#8901;", "&#8968;", "&#8969;", "&#8970;", "&#8971;", "&#9001;", "&#9002;", "&#9674;", "&#9824;", "&#9827;", "&#9829;", "&#9830;" };
            var chars = new int[] { 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 34, 38, 60, 62, 338, 339, 352, 353, 376, 710, 732, 8194, 8195, 8201, 8204, 8205, 8206, 8207, 8211, 8212, 8216, 8217, 8218, 8220, 8221, 8222, 8224, 8225, 8240, 8249, 8250, 8364, 402, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 977, 978, 982, 8226, 8230, 8242, 8243, 8254, 8260, 8472, 8465, 8476, 8482, 8501, 8592, 8593, 8594, 8595, 8596, 8629, 8656, 8657, 8658, 8659, 8660, 8704, 8706, 8707, 8709, 8711, 8712, 8713, 8715, 8719, 8721, 8722, 8727, 8730, 8733, 8734, 8736, 8743, 8744, 8745, 8746, 8747, 8756, 8764, 8773, 8776, 8800, 8801, 8804, 8805, 8834, 8835, 8836, 8838, 8839, 8853, 8855, 8869, 8901, 8968, 8969, 8970, 8971, 9001, 9002, 9674, 9824, 9827, 9829, 9830 };
            var decoded = new string[] { " ", "¡", "¢", "£", "¤", "¥", "¦", "§", "¨", "©", "ª", "«", "¬", "­", "®", "¯", "°", "±", "²", "³", "´", "µ", "¶", "·", "¸", "¹", "º", "»", "¼", "½", "¾", "¿", "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Ð", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "×", "Ø", "Ù", "Ú", "Û", "Ü", "Ý", "Þ", "ß", "à", "á", "â", "ã", "ä", "å", "æ", "ç", "è", "é", "ê", "ë", "ì", "í", "î", "ï", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "÷", "ø", "ù", "ú", "û", "ü", "ý", "þ", "ÿ", "\"", "&", "<", ">", "Œ", "œ", "Š", "š", "Ÿ", "ˆ", "˜", " ", " ", " ", "‌", "‍", "‎", "‏", "–", "—", "‘", "’", "‚", "“", "”", "„", "†", "‡", "‰", "‹", "›", "€", "ƒ", "Α", "Β", "Γ", "Δ", "Ε", "Ζ", "Η", "Θ", "Ι", "Κ", "Λ", "Μ", "Ν", "Ξ", "Ο", "Π", "Ρ", "Σ", "Τ", "Υ", "Φ", "Χ", "Ψ", "Ω", "α", "β", "γ", "δ", "ε", "ζ", "η", "θ", "ι", "κ", "λ", "μ", "ν", "ξ", "ο", "π", "ρ", "ς", "σ", "τ", "υ", "φ", "χ", "ψ", "ω", "ϑ", "ϒ", "ϖ", "•", "…", "′", "″", "‾", "⁄", "℘", "ℑ", "ℜ", "™", "ℵ", "←", "↑", "→", "↓", "↔", "↵", "⇐", "⇑", "⇒", "⇓", "⇔", "∀", "∂", "∃", "∅", "∇", "∈", "∉", "∋", "∏", "∑", "−", "∗", "√", "∝", "∞", "∠", "∧", "∨", "∩", "∪", "∫", "∴", "∼", "≅", "≈", "≠", "≡", "≤", "≥", "⊂", "⊃", "⊄", "⊆", "⊇", "⊕", "⊗", "⊥", "⋅", "⌈", "⌉", "⌊", "⌋", "⟨", "⟩", "◊", "♠", "♣", "♥", "♦" };
            var htm = html.ToString();
            for (int x = 0; x < html1.Length; x++)
            {
                htm = htm.Replace(html1[x], decoded[x]).Replace(html2[x], decoded[x]);
            }
            if (htm.IndexOf("&#") >= 0)
            {
                //replace remaining encoded characters
                var i = 0;
                var s = 0;
                var s2 = 0;
                var str = "";
                var character = "";
                do
                {
                    if (s2 >= htm.Length) { break; }
                    s = htm.IndexOf("&#", s2);
                    if (s >= 0)
                    {
                        s2 = htm.IndexOf(";", s + 1);
                        if (s2 > 0)
                        {
                            str = htm.Substring(s, (s2 + 1) - s);
                            character = Char.ConvertFromUtf32(int.Parse(str.Replace("&#", "").Replace(";", "")));
                            htm = htm.Replace(str, character);
                            i = 0;
                        }
                    }
                    else { break; }
                    i++;
                } while (s >= 0 && i < 3);
            }
            return htm;
        }

        public string RemoveHtmlFromString(string str, bool includeBR = false)
        {
            string RegExStr = "<[^>]*>";
            if (includeBR == true)
                RegExStr = "(\\<)(?!br(\\s|\\/|\\>))(.*?\\>)";
            Regex S = new Regex(RegExStr);
            return S.Replace(str, "");
        }

        public string MinifyJS(string js)
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
            //result = Regex.Replace(result, "\r\n", " ", RegexOptions.Compiled Or RegexOptions.ECMAScript)
            return result;
        }

        public string MaxChars(string str, int max, string trail = "")
        {
            if (str.Length > max)
            {
                return str.Substring(0, max) + trail;
            }
            return str;
        }

        public string RemoveApostrophe(string word)
        {
            var w = word
                .Replace("'s", "").Replace("’s", "").Replace("'t", "{{t}}").Replace("’t", "{{t}}")
                .Replace("'", "").Replace("’", "").Replace("{{t}}", "'t");
            return w;
        }

        #endregion

        #region "Extraction"
        public string getFileExtension(string filename)
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

        public string getFolder(string filename)
        {
            var paths = filename.Replace("/", "\\").Split('\\').ToList();
            paths.RemoveAt(paths.Count - 1);
            return string.Join("\\", paths.ToArray()) + "\\";
        }

        public string GetDomainName(string url)
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

        public string GetSubDomainAndDomain(string url)
        {
            string strDomain = url.Replace("http://", "").Replace("https://", "").Replace("www.", "").Split('/')[0];
            if (strDomain.IndexOf("localhost") >= 0 | strDomain.IndexOf("192.168") >= 0)
            {
                strDomain = "collector.com";
            }
            return strDomain.Replace("/", "");
        }

        public string[] GetDomainParts(string url)
        {
            string subdomain = GetSubDomainAndDomain(url);
            string domain = GetDomainName(subdomain);
            string sub = subdomain.Replace(domain, "").Replace(".", "");
            if (sub != "")
            {
                return new string[] { sub, subdomain.Replace(sub, "") };
            }
            return new string[] { "", subdomain };
        }

        public string GetPageTitle(string title)
        {
            return title.Split(new string[] { " - " }, 0)[1];
        }

        public string GetWebsiteTitle(string title)
        {
            return title.Split(new string[] { " - " }, 0)[0];
        }

        public double[] GetNumbersFromText(string text)
        {
            var isnum = false;
            var numpart = "";
            var spart = "";
            var numberlist = new List<double>();
            for (var x = 0; x < text.Length; x++)
            {
                spart = text.Substring(x, 1);
                if (Util.Str.IsNumeric(spart) || spart == ".")
                {
                    //found a number
                    if (isnum == false)
                    {
                        numpart = spart;
                    }
                    else
                    {
                        numpart += spart;
                    }
                }
                else if (spart == ".")
                {
                    //just a period
                    numpart = "";
                }
                else if (numpart != "")
                {
                    //end of number
                    if (Util.Str.IsNumeric(numpart))
                    {
                        numberlist.Add(double.Parse(numpart));
                    }

                    numpart = "";
                }
            }
            return numberlist.ToArray();
        }
        #endregion

        #region "Generation"
        public string CreateID(int length = 3)
        {
            string result = "";
            for (var x = 0; x <= length - 1; x++)
            {
                int type = Util.Random.Next(1, 3);
                int num = 0;
                switch (type)
                {
                    case 1: //a-z
                        num = Util.Random.Next(0, 26);
                        result += (char)('a' + num);
                        break;

                    case 2: //A-Z
                        num = Util.Random.Next(0, 26);
                        result += (char)('A' + num);
                        break;

                    case 3: //0-9
                        num = Util.Random.Next(0, 9);
                        result += (char)('1' + num);
                        break;

                }

            }
            return result;
        }

        public string CreateMD5Hash(string input)
        {
            var encryption = new Encryption(Util);
            return encryption.GetMD5Hash(input);
        }

        public string DateFolders(DateTime myDate, int folderCount = 3)
        {
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

        public string NumberSuffix(int digit)
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

        public string DateSentence(DateTime myDate, string dateSeparator = "-")
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

        #endregion

        #region "Validation"
        public bool IsNumeric(string str)
        {
            double retNum;
            if (Util.IsEmpty(str) == false)
            {
                return Double.TryParse(str, out retNum);
            }
            return false;
        }

        public bool OnlyAlphabet(string myStr, params string[] exceptionList)
        {
            bool result = false;
            for (int x = 0; x <= myStr.Length - 1; x++)
            {
                result = false;
                if (Asc(myStr.Substring(x, 1)) >= Asc("a") & Asc(myStr.Substring(x, 1)) <= Asc("z"))
                {
                    result = true;
                }
                if (Asc(myStr.Substring(x, 1)) >= Asc("A") & Asc(myStr.Substring(x, 1)) <= Asc("Z"))
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

        public bool OnlyLettersAndNumbers(string myStr, params string[] exceptionList)
        {
            bool result = false;
            for (int x = 0; x <= myStr.Length - 1; x++)
            {
                result = false;
                if (Asc(myStr.Substring(x, 1)) >= Asc("a") & Asc(myStr.Substring(x, 1)) <= Asc("z"))
                {
                    result = true;
                }

                if (Asc(myStr.Substring(x, 1)) >= Asc("A") & Asc(myStr.Substring(x, 1)) <= Asc("Z"))
                {
                    result = true;
                }

                if (Asc(myStr.Substring(x, 1)) >= Asc("0") & Asc(myStr.Substring(x, 1)) <= Asc("9"))
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

        public bool CheckChar(string character, bool allowAlpha = true, bool allowNumbers = true, string[] allowList = null, bool capitalizedOnly = false)
        {
            if (allowAlpha == true)
            {
                if (Asc(character) >= Asc("a") & Asc(character) <= Asc("z"))
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

                if (Asc(character) >= Asc("A") & Asc(character) <= Asc("Z"))
                {
                    return true;
                }
            }

            if (allowNumbers == true)
            {
                if (Asc(character) >= Asc("0") & Asc(character) <= Asc("9"))
                {
                    return true;
                }
            }

            if ((allowList != null))
            {
                foreach (string c in allowList)
                {
                    if (c == character)
                        return true;
                }
            }

            return false;
        }

        public bool ContainsCurseWords(string txt)
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

        public string CleanInput(string input, bool noHtml = true, bool noJs = true, bool noEncoding = true, bool noSpecialChars = true, string[] allowedChars = null)
        {
            //cleans any malacious patterns from the user input 
            string cleaned = input;
            return cleaned;
        }
        #endregion
    }
}
