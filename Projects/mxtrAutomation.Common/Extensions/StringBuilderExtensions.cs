using System;
using System.Linq;
using System.Text;

namespace mxtrAutomation.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        public const string TabSpaces = "    ";

        public static StringBuilder Tab(this StringBuilder sb)
        {
            return sb.Tab(1);
        }

        public static StringBuilder Tab(this StringBuilder sb, int count)
        {
            if (sb == null)
                sb = new StringBuilder();

            Enumerable.Range(0, count).ForEach(i => sb.Append(TabSpaces));
            return sb;
        }

        public static StringBuilder NewLine(this StringBuilder sb)
        {
            return sb.NewLine(1);
        }

        public static StringBuilder NewLine(this StringBuilder sb, int count)
        {
            if (sb == null)
                sb = new StringBuilder();

            Enumerable.Range(0, count).ForEach(i => sb.Append(Environment.NewLine));
            return sb;
        }
    }

}
