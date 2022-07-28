using System;
using System.Xml.Linq;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.XElementExtensionsTests
{
    public class NullableIntValueOrDefaultTests
    {
        private const string ElementTempalte = "<price>XXX</price>";

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", null)]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        public void GivenAValidInteger_NullableIntValueOrDefault_ReturnsAnInteger(string textValue, int? expectedValue)
        {
            // Arrange.
            var xElement = XElement.Parse(ElementTempalte.Replace("XXX", textValue));

            // Act.
            var result = xElement.NullableIntValueOrDefault();

            // Assert.
            result.ShouldBe(expectedValue);
        }

        [Fact]
        public void GivenAnInvalidInteger_NullableIntValueOrDefault_ReturnsAnInteger()
        {
            // Arrange.
            var xElement = XElement.Parse(ElementTempalte.Replace("XXX", "aaa"));

            // Act.
            var exception = Should.Throw<Exception>(() => xElement.NullableIntValueOrDefault());

            // Assert.
            exception.Message.ShouldBe("Failed to parse element: price; value: 'aaa' into a int.");
        }
    }
}
