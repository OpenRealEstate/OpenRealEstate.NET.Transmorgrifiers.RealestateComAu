using System;


namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu
{
    internal static class Guard
    {
        internal static void AgainstNull(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
        }

        internal static void AgainstNullOrWhiteSpace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
