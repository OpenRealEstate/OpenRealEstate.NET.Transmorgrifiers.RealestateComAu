using System;
using System.IO;
using System.Linq;
using OpenRealEstate.Core;
using OpenRealEstate.Core.Rural;
using OpenRealEstate.FakeData;
using OpenRealEstate.Testing;
using OpenRealEstate.Transmorgrifiers.Core;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests
{
    public class ParseRuralTests
    {
        private const string FakeDataFolder = "Sample Data/Rural/";

        private static RuralListing CreateAFakeEmptyRuralListing(string id)
        {
            id.ShouldNotBeNullOrWhiteSpace();

            return new RuralListing
            {
                AgencyId = "XNWXNW",
                Id = id,
                CreatedOn = new DateTime(2009, 1, 1, 12, 30, 0),
                UpdatedOn = new DateTime(2009, 1, 1, 12, 30, 0)
            };
        }

        private static void AssertRuralListing(ParsedResult result,
                                               RuralListing expectedListing)
        {
            result.ShouldNotBeNull();
            result.Listings.Count.ShouldBe(1);
            result.Errors.Count.ShouldBe(0);
            result.UnhandledData.Count.ShouldBe(0);
            RuralListingAssertHelpers.AssertRuralListing(result.Listings.First().Listing as RuralListing,
                                                         expectedListing);
        }

        [Theory]
        [InlineData("REA-Rural-Current.xml")]
        [InlineData("REA-Segment-Rural-Current.xml")]
        public void GivenTheFileREARuralCurrent_Parse_ReturnsARuralAvailableListing(string fileName)
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeRuralListing();
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRuralListing(result, expectedListing);
        }

        [Theory]
        [InlineData("REA-Rural-Withdrawn.xml", "withdrawn", "Rural-Withdrawn-ABCD1234")]
        [InlineData("REA-Rural-OffMarket.xml", "offmarket", "Rural-OffMarket-ABCD1234")]
        [InlineData("REA-Rural-Deleted.xml", "deleted", "Rural-Deleted-ABCD1234")]
        public void GivenAnReaRuralFileThatRepresentsARemovedListing_Parse_ReturnsARemovedListing(string fileName,
                                                                                                  string sourceStatus,
                                                                                                  string id)
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyRuralListing(id);
            expectedListing.StatusType = StatusType.Removed;
            expectedListing.SourceStatus = sourceStatus;
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRuralListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREARuralSoldDisplayPriceisNo_Parse_ReturnsARuralSoldListing()
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyRuralListing("Rural-Sold-ABCD1234");
            expectedListing.StatusType = StatusType.Sold;
            expectedListing.SourceStatus = "sold";
            expectedListing.Pricing = new SalePricing
            {
                SoldOn = new DateTime(2009, 1, 10, 12, 30, 00),
                SoldPrice = 85000
            };
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rural-Sold-DisplayPriceisNo.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRuralListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREARuralSolder_Parse_ReturnsARemovedListing()
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyRuralListing("Rural-Sold-ABCD1234");
            expectedListing.StatusType = StatusType.Sold;
            expectedListing.SourceStatus = "sold";
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rural-Sold.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRuralListing(result, expectedListing);
        }
    }
}
