using System;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.SystemExtensionsTests
{
    public class ParseByteValueOrDefaultTests
    {
        [Theory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("1", 1)]
        [InlineData("1.0", 1)]
        public void GivenAValidValue_ParseByteValueOrDefault_ReturnsAByte(string value, byte expectedResult)
        {
            // Arrange && Act.
            var result = value.ParseByteValueOrDefault();

            // Assert.
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("-1")]
        [InlineData("1.1")]
        public void GivenAnInvalidValue_ParseByteValueOrDefault_ThrowAnException(string value)
        {
            // Arrange && Act.
            var exception = Should.Throw<Exception>(() => value.ParseByteValueOrDefault());

            // Assert.
            exception.Message.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
