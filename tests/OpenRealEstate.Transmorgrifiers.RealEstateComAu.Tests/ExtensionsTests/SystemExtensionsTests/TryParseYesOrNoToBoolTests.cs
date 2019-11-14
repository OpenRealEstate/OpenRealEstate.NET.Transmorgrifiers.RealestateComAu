using System;
using OpenRealEstate.Transmorgrifiers.RealEstateComAu.Extensions;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests.ExtensionsTests.SystemExtensionsTests
{
    public class TryParseYesOrNoToBoolTests
    {
        [Theory]
        [InlineData("true", true)]
        [InlineData("TrUe", true)] // Ignore the casing.
        [InlineData("false", false)]
        [InlineData("FaLsE", false)] // Ignore the casing.
        [InlineData("aaaa", false)]
        public void GivenAValidValue_TryParseYesTrueOrNoFalseToBool_ReturnsTrueOrFalse(string value, bool expectedResult)
        {
            // Arrange & Act.
            value.TryParseYesOrNoToBool(out var result);

            // Assert.
            result.ShouldBe(expectedResult);
        }
    }
}
