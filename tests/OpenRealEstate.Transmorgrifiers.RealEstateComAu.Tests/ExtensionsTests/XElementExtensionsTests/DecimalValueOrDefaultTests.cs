using System;
using System.Xml.Linq;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.XElementExtensionsTests
{
    public class DecimalValueOrDefaultTests
    {
        [Theory]
        [InlineData(null, 0)]
        [InlineData(" ", 0)]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        public void GivenAValidInteger_DecimalValueOrDefault_ReturnsAnInteger(string textValue, decimal expectedValue)
        {
            // Arrange.
            var xElement = XElement.Parse(XElementExtensionsTestHelpers.ElementTemplate.Replace("XXX", textValue));

            // Act.
            var result = xElement.DecimalValueOrDefault();

            // Assert.
            result.ShouldBe(expectedValue);
        }

        [Fact]
        public void GivenAnInvalidInteger_DecimalValueOrDefault_ReturnsAnInteger()
        {
            // Arrange.
            var xElement = XElement.Parse(XElementExtensionsTestHelpers.ElementTemplate.Replace("XXX", "aaa"));

            // Act.
            var exception = Should.Throw<Exception>(() => xElement.DecimalValueOrDefault());

            // Assert.
            exception.Message.ShouldBe("Failed to parse element: price; value: 'aaa' into a decimal.");
        }
    }
}
