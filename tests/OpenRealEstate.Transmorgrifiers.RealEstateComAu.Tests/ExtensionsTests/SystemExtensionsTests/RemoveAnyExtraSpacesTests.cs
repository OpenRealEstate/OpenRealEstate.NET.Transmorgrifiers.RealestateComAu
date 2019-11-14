using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.SystemExtensionsTests
{
    public class RemoveAnyExtraSpacesTests
    {
        [Theory]
        [InlineData("a", "a")]
        [InlineData(" a", "a")]
        [InlineData(" a ", "a")]
        [InlineData("a   ", "a")]
        [InlineData("   a", "a")]
        [InlineData("   a   ", "a")]
        [InlineData("a   ", "a")]
        [InlineData(" aaa ", "aaa")]
        [InlineData(" a b c d e f ", "a b c d e f")]
        [InlineData("  a  b  c  d  e  f ", "a b c d e f")]
        [InlineData(" aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa                                                           a", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa a")]
        public void GivenAString_RemoveAnyExtraSpaces_ReturnsACleanedString(string sourceText, string expectedText)
        {
            // Arrange & Act.
            var result = sourceText.RemoveAnyExtraSpaces();

            // Assert.
            result.ShouldBe(expectedText);
        }
    }
}
