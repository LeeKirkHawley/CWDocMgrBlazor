using SharedLib.Enums;

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

        public static string StripExtensionFromImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            // Get all enum values as strings
            string[] extensions = Enum.GetNames(typeof(SupportedImageExtensions))
                .Select(e => "." + e.ToLowerInvariant())
                .ToArray();

            foreach (string extension in extensions)
            {
                if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    // Remove the extension
                    return fileName.Substring(0, fileName.Length - extension.Length);
                }
            }

            return fileName;
        }

        public static string GetAllowedExtensionFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            // Get all enum values as strings
            string[] extensions = Enum.GetNames(typeof(SupportedImageExtensions))
                .Select(e => "." + e.ToLowerInvariant())
                .ToArray();

            foreach (string extension in extensions)
            {
                if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    // Remove the extension
                    return extension;
                }
            }

            return "";
        }


    }
}
