using System.Text;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.StringBuilderExtensionsTests
{
    public class PrependWithDelimeterTests
    {
        [Theory]
        [InlineData(null, "aaa", null, "aaa")] // No initial text, so no delimeter needed.
        [InlineData("aaa", "bbb", null, "aaa, bbb")] // Has text so delimeter needed.
        [InlineData("aaa", "bbb", "---", "aaa---bbb")] // Has text and a new delimeter, so provided delimeter needed.
        public void GivenSomeText_PrependWithDelimeter_ReturnsAValidString(string initialText,
                                                                              string additionalText,
                                                                              string delimeter,
                                                                              string expectedText)
        {
            // Arrange.
            var stringBuilder = new StringBuilder(initialText);

            // Act.
            stringBuilder.PrependWithDelimeter(additionalText, delimeter);

            // Assert.
            stringBuilder.ToString().ShouldBe(expectedText);
        }
    }
}
