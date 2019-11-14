using System.Text;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions
{
    internal static class StringBuilderExtensions
    {
        private const string DefaultDelimeter = ", ";

        internal static void PrependWithDelimeter(this StringBuilder stringBuilder,
                                                  string value,
                                                  string delimeter = DefaultDelimeter)
        {
            Guard.AgainstNullOrWhiteSpace(value);

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(string.IsNullOrWhiteSpace(delimeter) ? DefaultDelimeter : delimeter);
            }

            stringBuilder.Append(value);
        }
    }
}
