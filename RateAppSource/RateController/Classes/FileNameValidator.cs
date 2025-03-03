using System;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    public static class FileNameValidator
    {
        // List of reserved names
        private static readonly string[] ReservedNames =
        {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        public static bool IsValidFileName(string name)
        {
            // Check for null or empty
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            // Check for reserved names
            if (ReservedNames.Contains(name.ToUpper()))
            {
                return false;
            }

            // Check for invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return name.All(c => !invalidChars.Contains(c));
        }

        public static bool IsValidFolderName(string name)
        {
            // Check for null or empty
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            // Check for reserved names
            if (ReservedNames.Contains(name.ToUpper()))
            {
                return false;
            }

            // Check for invalid characters
            char[] invalidChars = Path.GetInvalidPathChars();
            return name.All(c => !invalidChars.Contains(c));
        }
    }
}