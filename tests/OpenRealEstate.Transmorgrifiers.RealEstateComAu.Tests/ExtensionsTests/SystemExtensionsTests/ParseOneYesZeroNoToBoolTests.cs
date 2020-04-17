using System;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.SystemExtensionsTests
{
    public class ParseOneYesZeroNoToBoolTests
    {
        [Theory]
        [InlineData("1", true)]
        [InlineData("yes", true)]
        [InlineData("YeS", true)] // Ignore the casing.
        [InlineData("0", false)]
        [InlineData("no", false)]
        [InlineData("NO", false)] // Ignore the casing.
        public void GivenAValidValue_ParseOneYesZeroNoToBool_ReturnsABool(string value, bool expectedResult)
        {
            // Arrange & Act.
            var result = value.ParseOneYesZeroNoToBool();

            // Assert.
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("11")]
        [InlineData("-1")]
        [InlineData("aaa")]
        public void GivenAnInvalidValue_ParseOneYesZeroNoToBool_ThrowsAnException(string value)
        {
            // Arrange & Act.
            var exception = Should.Throw<ArgumentOutOfRangeException>( () => value.ParseOneYesZeroNoToBool());

            // Assert.
            exception.Message.ShouldBe($"Value '{value}' is out of range. It should only be 0/1/yes/no. (Parameter 'value')");
        }
    }
}
