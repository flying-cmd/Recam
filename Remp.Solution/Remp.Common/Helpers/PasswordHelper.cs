using Remp.Common.Utilities;
using System.Text;

namespace Remp.Common.Helpers;

public static class PasswordHelper
{
    /// <summary>
    /// Generate random password of a given length.
    /// The password contains at least one upper case letter, one lower case letter, one number and one special character.
    /// </summary>
    /// <param name="length">
    /// The length of the password
    /// </param>
    /// <returns>
    /// A random password
    /// </returns>
    public static string GenerateRandomPassword(int length = 8)
    {
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string numberChars = "0123456789";
        const string specialChars = "!@#$%^&*()_+";

        var allChars = upperChars + lowerChars + numberChars + specialChars;

        var password = new StringBuilder(length);

        var random = new Random();
        password.Append(upperChars[random.Next(0, upperChars.Length)]);
        password.Append(lowerChars[random.Next(0, lowerChars.Length)]);
        password.Append(numberChars[random.Next(0, numberChars.Length)]);
        password.Append(specialChars[random.Next(0, specialChars.Length)]);

        while (password.Length < length)
        {
            password.Append(allChars[random.Next(0, allChars.Length)]);
        }

        return password.ToString().Shuffle();
    }
}
