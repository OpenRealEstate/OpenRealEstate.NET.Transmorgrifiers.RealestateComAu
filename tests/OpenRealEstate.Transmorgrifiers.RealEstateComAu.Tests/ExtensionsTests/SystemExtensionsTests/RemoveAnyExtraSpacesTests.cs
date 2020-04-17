using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.SystemExtensionsTests
{
    public class RemoveExtraCharsBetweenWordsTests
    {
        [Theory]
        [InlineData("a", "a")]
        [InlineData(" a", "a")]
        [InlineData(" a ", "a")]
        [InlineData("a   ", "a")]
        [InlineData("   a", "a")]
        [InlineData("   a   ", "a")]
        [InlineData(" aaa ", "aaa")]
        [InlineData(" a b c d e f ", "a b c d e f")]
        [InlineData("  a  b  c  d  e  f ", "a b c d e f")]
        [InlineData(" aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa                                                           a", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa a")]
        [InlineData(" abc  \n cde", "abc cde")]
        [InlineData(" abc  \r cde ", "abc cde")]
        [InlineData("  abc   \t \n cde  ", "abc cde")]
        public void GivenAString_RemoveExtraCharsBetweenWords_ReturnsACleanedString(string sourceText, string expectedText)
        {
            // Arrange & Act.
            var result = sourceText.RemoveExtraCharsBetweenWords();

            // Assert.
            result.ShouldBe(expectedText);
        }
    }
}
