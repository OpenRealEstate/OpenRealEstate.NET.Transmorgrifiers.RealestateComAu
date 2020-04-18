using System.Globalization;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests
{
    public abstract class SetupTests
    {
        public SetupTests()
        {
            // Linux systems might not have any Culture setup by default. As such, currency tests might fail.
            // REF: https://www.hanselman.com/blog/AssertYourAssumptionsNETCoreAndSubtleLocaleIssuesWithWSLsUbuntu.aspx
            // REF: https://andrewlock.net/dotnet-core-docker-and-cultures-solving-culture-issues-porting-a-net-core-app-from-windows-to-linux/

            // So let's default _every_ test to a specific culture.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-au");
        }
    }
}
