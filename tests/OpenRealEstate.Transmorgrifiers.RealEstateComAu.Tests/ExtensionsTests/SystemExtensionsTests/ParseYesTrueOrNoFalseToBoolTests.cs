using System;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.SystemExtensionsTests
{
    public class ParseYesTrueOrNoFalseToBoolTests
    {
        [Theory]
        [InlineData("true", true)]
        [InlineData("TrUe", true)] // Ignore the casing.
        [InlineData("false", false)]
        [InlineData("FaLsE", false)] // Ignore the casing.
        public void GivenAValidValue_ParseYesTrueOrNoFalseToBool_ReturnsTrueOrFalse(string value, bool expectedResult)
        {
            // Arrange & Act.
            var result = value.ParseYesTrueOrNoFalseToBool();

            // Assert.
            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void GivenAnInvalidValue_ParseYesTrueOrNoFalseToBool_ThrowsAnException()
        {
            // Arrange & Act.
            var exception = Should.Throw<ArgumentOutOfRangeException>( () => "aaa".ParseYesTrueOrNoFalseToBool());

            // Assert.
            exception.Message.ShouldBe("Specified argument was out of the range of valid values. (Parameter 'value')");
        }
    }
}
