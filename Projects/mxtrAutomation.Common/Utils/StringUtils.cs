using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Utils
{
    public static class StringUtils
    {
        private static readonly string[] ProperCaseExceptions = { "a", "is", "and", "or" };
        private static readonly Regex _properNameRx = new Regex(@"\b(\w+)\b");
        private static readonly string[] _prefixes = { "mc" };
        private static readonly string[] _suffixesToLower = { "'S" };
        private static readonly string[] _suffixesToUpper = { "Llc" };
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly Random _randomSeed = new Random();

        public static string ConvertWordToProperCase(string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;

            word = word.ToLower().Trim();

            if (ProperCaseExceptions.Contains(word))
                return word;

            string result = _properNameRx.Replace(word.ToLower(CultureInfo.CurrentCulture), HandleWord);

            foreach (string suffix in _suffixesToLower)
            {
                result = result.Replace(suffix, suffix.ToLower());
            }
            foreach (string suffix in _suffixesToUpper)
            {
                result = result.Replace(suffix, suffix.ToUpper());
            }

            return result;
        }

        public static string WordToProperCase(this string word)
        {
            if (word.Length > 1)
                return Char.ToUpper(word[0], CultureInfo.CurrentCulture) + word.Substring(1);

            return word.ToUpper(CultureInfo.CurrentCulture);
        }

        private static string HandleWord(Match m)
        {
            string word = m.Groups[1].Value;
            foreach (string prefix in _prefixes)
            {
                if (word.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                    return prefix.WordToProperCase() + word.Substring(prefix.Length).WordToProperCase();
            }

            return word.WordToProperCase();
        }

        public static string ConvertWordToPossessive(string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;

            if (word.EndsWith("s"))
            {
                return word + "'";
            }
            else
            {
                return word + "'s";
            }

        }

        private static readonly IList<string> Unpluralizables = new List<string> { "equipment", "information", "rice", "money", "species", "series", "fish", "sheep", "deer" };

        private static readonly IDictionary<string, string> Pluralizations = new Dictionary<string, string>
        {
            // Start with the rarest cases, and move to the most common
            { "person", "people" },
            { "ox", "oxen" },
            { "child", "children" },
            { "foot", "feet" },
            { "tooth", "teeth" },
            { "goose", "geese" },
            { "form", "forms" },
            // And now the more standard rules.
            { "(.*)fe?", "$1ves" },         // ie, wolf, wife
            { "(.*)man$", "$1men" },
            { "(.+[aeiou]y)$", "$1s" },
            { "(.+[^aeiou])y$", "$1ies" },
            { "(.+z)$", "$1zes" },
            { "([m|l])ouse$", "$1ice" },
            { "(.+)(e|i)x$", @"$1ices"},    // ie, Matrix, Index
            { "(octop|vir)us$", "$1i"},
            { "(.+(s|x|sh|ch))$", @"$1es"},
            { "(.+)", @"$1s" }
        };

        public static string Pluralize(int count, string singular)
        {
            if (count == 1)
                return singular;

            if (Unpluralizables.Contains(singular))
                return singular;

            return
                Pluralizations
                    .Where(p => Regex.IsMatch(singular, p.Key))
                    .Select(p => Regex.Replace(singular, p.Key, p.Value))
                    .FirstOrDefault();
        }

        public static string RemoveDiacritics(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            s = s.MapUnicodeDiacritics();

            var strFormD = s.Normalize(NormalizationForm.FormD);
            var sb = new char[strFormD.Length * 2];
            var pos = 0;

            for (var i = 0; i < strFormD.Length; i++)
            {
                var c = strFormD[i];
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if ((uc != UnicodeCategory.NonSpacingMark) && (uc != UnicodeCategory.ModifierLetter))
                    sb[pos++] = c;
            }

            s = new string(sb, 0, pos).Normalize(NormalizationForm.FormC);

            return s;
        }

        /// <summary>
        /// Manually Maps Unicode characters with diacritic marks to their UTF-8 equivalent.
        /// </summary>
        public static string MapUnicodeDiacritics(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            //var sb = new StringBuilder();
            var sb = new char[s.Length * 2];

            // Known Character Mappings (UTF-16)
            var pos = 0;
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                var a = (int)c;
                var n = c.ToString();
                switch (a)
                {
                    case 0x100:
                    case 0x102:
                    case 0x104:
                    case 0x1CD:
                    case 0x1FA:
                    case 0x1EA0:
                    case 0x1EA2:
                    case 0x1EA4:
                    case 0x1EA6:
                    case 0x1EA8:
                    case 0x1EAA:
                    case 0x1EAC:
                    case 0x1EAE:
                    case 0x1EB0:
                    case 0x1EB2:
                    case 0x1EB4:
                    case 0x1EB6:
                        n = "A";
                        break;
                    case 0x01FC:
                        n = "AE";
                        break;
                    case 0x1FD:
                        n = "ae";
                        break;
                    case 0x101:
                    case 0x103:
                    case 0x105:
                    case 0x1CE:
                    case 0x1FB:
                    case 0x1EA5:
                    case 0x1EA7:
                    case 0x1EA9:
                    case 0x1EAB:
                    case 0x1EAD:
                    case 0x1EAF:
                    case 0x1EB1:
                    case 0x1EB3:
                    case 0x1EB5:
                    case 0x1EB7:
                        n = "a";
                        break;
                    case 0x106:
                    case 0x108:
                    case 0x10A:
                    case 0x10C:
                        n = "C";
                        break;
                    case 0x107:
                    case 0x109:
                    case 0x10B:
                    case 0x10D:
                        n = "c";
                        break;
                    case 0x10E:
                    case 0x110:
                        n = "D";
                        break;
                    case 0x10F:
                    case 0x111:
                        n = "d";
                        break;
                    case 0x112:
                    case 0x114:
                    case 0x116:
                    case 0x118:
                    case 0x11A:
                    case 0x1EB8:
                    case 0x1EBA:
                    case 0x1EBC:
                    case 0x1EBE:
                    case 0x1EC0:
                    case 0x1EC2:
                    case 0x1EC4:
                    case 0x1EC6:
                        n = "E";
                        break;
                    case 0x113:
                    case 0x115:
                    case 0x117:
                    case 0x119:
                    case 0x11B:
                    case 0x18F:
                    case 0x259:
                    case 0x1EB9:
                    case 0x1EBB:
                    case 0x1EBD:
                    case 0x1EBF:
                    case 0x1EC1:
                    case 0x1EC3:
                    case 0x1EC5:
                    case 0x1EC7:
                        n = "e";
                        break;
                    case 0x192:
                        n = "f";
                        break;
                    case 0x11C:
                    case 0x11E:
                    case 0x120:
                    case 0x122:
                        n = "G";
                        break;
                    case 0x11D:
                    case 0x11F:
                    case 0x121:
                    case 0x123:
                        n = "g";
                        break;
                    case 0x124:
                    case 0x126:
                        n = "H";
                        break;
                    case 0x125:
                    case 0x127:
                        n = "h";
                        break;
                    case 0x128:
                    case 0x12A:
                    case 0x12C:
                    case 0x12E:
                    case 0x130:
                    case 0x1CF:
                    case 0x1EC8:
                    case 0x1ECA:
                        n = "I";
                        break;
                    case 0x129:
                    case 0x12B:
                    case 0x12D:
                    case 0x12F:
                    case 0x131:
                    case 0x1D0:
                    case 0x1EC9:
                    case 0x1ECB:
                        n = "i";
                        break;
                    case 0x132:
                        n = "IJ";
                        break;
                    case 0x133:
                        n = "ij";
                        break;
                    case 0x134:
                        n = "J";
                        break;
                    case 0x135:
                        n = "j";
                        break;
                    case 0x136:
                        n = "K";
                        break;
                    case 0x137:
                    case 0x138:
                        n = "k";
                        break;
                    case 0x139:
                    case 0x13B:
                    case 0x13D:
                    case 0x13F:
                    case 0x141:
                        n = "L";
                        break;
                    case 0x13A:
                    case 0x13C:
                    case 0x13E:
                    case 0x140:
                    case 0x142:
                        n = "l";
                        break;
                    case 0x143:
                    case 0x145:
                    case 0x147:
                    case 0x14A:
                        n = "N";
                        break;
                    case 0x144:
                    case 0x146:
                    case 0x148:
                    case 0x149:
                    case 0x14B:
                        n = "n";
                        break;
                    case 0x14C:
                    case 0x14E:
                    case 0x150:
                    case 0x1A0:
                    case 0x1D1:
                    case 0x1FE:
                    case 0x1ECC:
                    case 0x1ECE:
                    case 0x1ED0:
                    case 0x1ED2:
                    case 0x1ED4:
                    case 0x1ED6:
                    case 0x1ED8:
                    case 0x1EDA:
                    case 0x1EDC:
                    case 0x1EDE:
                    case 0x1EE0:
                    case 0x1EE2:
                        n = "O";
                        break;
                    case 0x14D:
                    case 0x14F:
                    case 0x151:
                    case 0x1A1:
                    case 0x1D2:
                    case 0x1FF:
                    case 0x1ECD:
                    case 0x1ECF:
                    case 0x1ED1:
                    case 0x1ED3:
                    case 0x1ED5:
                    case 0x1ED7:
                    case 0x1ED9:
                    case 0x1EDB:
                    case 0x1EDD:
                    case 0x1EDF:
                    case 0x1EE1:
                    case 0x1EE3:
                        n = "o";
                        break;
                    case 0x152:
                        n = "OE";
                        break;
                    case 0x153:
                        n = "oe";
                        break;
                    case 0x154:
                    case 0x156:
                    case 0x158:
                        n = "R";
                        break;
                    case 0x155:
                    case 0x157:
                    case 0x159:
                        n = "r";
                        break;
                    case 0x15A:
                    case 0x15C:
                    case 0x15E:
                    case 0x160:
                        n = "S";
                        break;
                    case 0x15B:
                    case 0x15D:
                    case 0x15F:
                    case 0x161:
                    case 0x17F:
                        n = "s";
                        break;
                    case 0x162:
                    case 0x164:
                    case 0x166:
                        n = "T";
                        break;
                    case 0x163:
                    case 0x165:
                    case 0x167:
                        n = "t";
                        break;
                    case 0x168:
                    case 0x16A:
                    case 0x16C:
                    case 0x16E:
                    case 0x170:
                    case 0x172:
                    case 0x1AF:
                    case 0x1D3:
                    case 0x1D5:
                    case 0x1D7:
                    case 0x1D9:
                    case 0x1DB:
                    case 0x1EE4:
                    case 0x1EE6:
                    case 0x1EE8:
                    case 0x1EEA:
                    case 0x1EEC:
                    case 0x1EEE:
                    case 0x1EF0:
                        n = "U";
                        break;
                    case 0x169:
                    case 0x16B:
                    case 0x16D:
                    case 0x16F:
                    case 0x171:
                    case 0x173:
                    case 0x1B0:
                    case 0x1D4:
                    case 0x1D6:
                    case 0x1D8:
                    case 0x1DA:
                    case 0x1DC:
                    case 0x1EE5:
                    case 0x1EE7:
                    case 0x1EE9:
                    case 0x1EEB:
                    case 0x1EED:
                    case 0x1EEF:
                    case 0x1EF1:
                        n = "u";
                        break;
                    case 0x174:
                    case 0x1E80:
                    case 0x1E82:
                    case 0x1E84:
                        n = "W";
                        break;
                    case 0x175:
                    case 0x1E81:
                    case 0x1E83:
                    case 0x1E85:
                        n = "w";
                        break;
                    case 0x176:
                    case 0x178:
                    case 0x1EF2:
                    case 0x1EF4:
                    case 0x1EF6:
                    case 0x1EF8:
                        n = "Y";
                        break;
                    case 0x177:
                    case 0x1EF3:
                    case 0x1EF5:
                    case 0x1EF7:
                    case 0x1EF9:
                        n = "y";
                        break;
                    case 0x179:
                    case 0x17B:
                    case 0x17D:
                        n = "Z";
                        break;
                    case 0x17A:
                    case 0x17C:
                    case 0x17E:
                        n = "z";
                        break;
                }
                for (var j = 0; j < n.Length; j++)
                    sb[pos++] = n[j];
            }
            return new string(sb, 0, pos);
        }

        /// <summary>
        /// Computes the distance between 2 strings using the Levenshtein distance algorithm.
        /// Credit: http://dotnetperls.com/levenshtein
        /// </summary>
        public static int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static string Teaser(string sentence, int length)
        {
            if (sentence == null || sentence.Length <= length)
                return sentence;

            string truncated = sentence.Substring(0, length);
            int indexOfLastSpace = truncated.LastIndexOf(' ');

            return ((indexOfLastSpace >= 0) ? truncated.Substring(0, truncated.LastIndexOf(' ')) : truncated) + " ...";
        }

        public static string StripHtml(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
                return htmlText;

            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(htmlText, "");
        }

        public static string RandomStringGenerator(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_randomSeed.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public static string RandomNumberStringGenerator(int size)
        {
            string charPool = "0123456789";
            StringBuilder rs = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                rs.Append(charPool[(int)(random.NextDouble() * charPool.Length)]);
            }
            return rs.ToString();
        }

        public static string GenerateUniqueStringID()
        {
            long ms = DateTimeExtensions.ToUnixTimeStampMilliseconds(DateTime.Now);
            return string.Format("{0}{1}", ms, RandomStringGenerator(6));
        }

        public static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        public static string GenerateUserID()
        {
            long ms = DateTimeExtensions.ToUnixTimeStampMilliseconds(DateTime.Now);
            return string.Format("{0}{1}", ms, RandomNumberStringGenerator(2));
        }

        /// <summary>
        ///  Removes all special characters from a string
        /// </summary>
        public static string ToSlug(this string text)
        {
            StringBuilder sb = new StringBuilder();
            var lastWasInvalid = false;
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                    lastWasInvalid = false;
                }
                else
                {
                    if (!lastWasInvalid)
                        sb.Append("-");
                    lastWasInvalid = true;
                }
            }

            return sb.ToString().ToLowerInvariant().Trim();

        }


        /// <summary>
        ///  Converts an external url web query to a mixed content friendly url string
        /// </summary>
        public static string ToMixedContentFriendlyUrlString(this string externalWebQueryUrl, string protocol)
        {
            Uri url;
            bool result = Uri.TryCreate(externalWebQueryUrl, UriKind.Absolute, out url);
            if (result)
            {
                return string.Format("{0}://{1}{2}", protocol, url.Authority, url.PathAndQuery);
            }
            else
            {
                return externalWebQueryUrl;
            }

        }

        /// <summary>
        ///  Converts a comma delimited string of Guids to a List of Guids
        /// </summary>
        public static List<Guid> SplitStringToGuids(string param, char delimiter)
        {
            List<Guid> seperatedIDs = (from guid in param.Split(delimiter) select new Guid(guid)).ToList();
            return seperatedIDs;
        }

        /// <summary>
        ///  Converts a comma delimited string of ints to a List of ints
        /// </summary>
        public static List<int> SplitStringToIntegers(string param, char delimiter)
        {
            List<int> seperatedIDs = (from s in param.Split(delimiter) select int.Parse(s)).ToList();
            return seperatedIDs;
        }

        public static List<long> ConvertStringListToLongList(List<string> items)
        {
            List<long> filteredItems = new List<long>();
            long newItem = 0;

            foreach (string item in items)
            {
                if (long.TryParse(item, out newItem))
                    filteredItems.Add(newItem);
            }
            return filteredItems;
        }

    }
}
