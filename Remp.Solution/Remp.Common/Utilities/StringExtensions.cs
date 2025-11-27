namespace Remp.Common.Utilities;

public static class StringExtensions
{
    public static string Shuffle(this string str)
    {
        char[] chars = str.ToCharArray();
        var random = new Random();
        for (var i = chars.Length - 1; i > 0; i--)
        {
            // Generate random index between 0 and i
            var j = random.Next(i);

            // Swap elements i and j
            var temp = chars[i];
            chars[i] = chars[j];
            chars[j] = temp;
        }
        return new string(chars);
    }
}
