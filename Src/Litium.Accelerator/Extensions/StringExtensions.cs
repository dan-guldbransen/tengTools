namespace Litium.Accelerator.Extensions
{
    public static class StringExtensions
    {
        public static string NullIfWhiteSpace(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value;
        }
    }
}
