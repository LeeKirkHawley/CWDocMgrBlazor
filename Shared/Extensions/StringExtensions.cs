using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<int> AllIndexesOf(this string text, char character)
        {
            if (string.IsNullOrEmpty(text))
                yield break;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == character)
                    yield return i;
            }
        }
    }
}
