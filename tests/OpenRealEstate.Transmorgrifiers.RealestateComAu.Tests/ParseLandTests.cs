using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRealEstate.Core;
using OpenRealEstate.Core.Land;
using OpenRealEstate.FakeData;
using OpenRealEstate.Testing;
using OpenRealEstate.Transmorgrifiers.Core;
using Shouldly;
using Xunit;
using LandRuralCategoryType = OpenRealEstate.Core.Land.CategoryType;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests
{
    public class ParseLandTests
    {
        private const string FakeDataFolder = "Sample Data\\Land\\";

        private static LandListing CreateAFakeEmptyLandListing(string id)
        {
            id.ShouldNotBeNullOrWhiteSpace();

            return new LandListing
            {
                AgencyId = "XNWXNW",
                Id = id,
                CreatedOn = new DateTime(2009, 1, 1, 12, 30, 0),
                UpdatedOn = new DateTime(2009, 1, 1, 12, 30, 0)
            };
        }

        private static void AssertLandListing(ParsedResult result,
                                              LandListing expectedListing)
        {
            result.ShouldNotBeNull();
            result.Listings.Count.ShouldBe(1);
            result.Errors.Count.ShouldBe(0);
            result.UnhandledData.Count.ShouldBe(0);
            LandListingAssertHelpers.AssertLandListing(result.Listings.First().Listing as LandListing,
                                                       expectedListing);
        }

        [Theory]
        [InlineData("REA-Land-Current.xml")]
        [InlineData("REA-Segment-Land-Current.xml")]
        public void GivenTheFileREALandCurrent_Parse_ReturnsALandAvailableListing(string fileName)
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeLandListing();
            expectedListing.Address.StreetNumber = "LOT 12/39";
            expectedListing.Address.DisplayAddress = expectedListing.Address.ToFormattedAddress(true, StateReplacementType.ReplaceToLongText, false, true); 

            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertLandListing(result, expectedListing);
        }

        [Theory]
        [InlineData("REA-Land-Sold.xml", "$85,000")]
        [InlineData("REA-Land-Sold-DisplayPriceIsNo.xml", "aaaa")]
        [InlineData("REA-Land-Sold-DisplayPriceIsNo.xml", null)]
        public void GivenAnREALandSoldFile_Parse_ReturnsARemovedListing(string fileName,
                                                                        string expectedSoldPriceText)
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyLandListing("Land-Sold-ABCD1234");
            expectedListing.StatusType = StatusType.Sold;
            expectedListing.SourceStatus = "Sold";
            expectedListing.Pricing = new SalePricing
            {
                SoldPrice = 85000,
                SoldPriceText = expectedSoldPriceText,
                SoldOn = new DateTime(2009, 1, 10, 12, 30, 00)
            };
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);

            // NOTE: Lets use the expected text as the default to make sure this value will be used in the logic.
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier
            {
                DefaultSoldPriceTextIfMissing = expectedSoldPriceText 
            };

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertLandListing(result, expectedListing);
        }

        [Theory]
        [InlineData("REA-Land-Withdrawn.xml", "withdrawn", "Land-Withdrawn-ABCD1234")]
        [InlineData("REA-Land-OffMarket.xml", "offmarket", "Land-OffMarket-ABCD1234")]
        [InlineData("REA-Land-Deleted.xml", "deleted", "Land-Deleted-ABCD1234")]
        public void GivenAnReaLandFileThatRepresentsARemovedListing_Parse_ReturnsARemovedListing(string fileName,
                                                                                                 string sourceStatus,
                                                                                                 string id)
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyLandListing(id);
            expectedListing.StatusType = StatusType.Removed;
            expectedListing.SourceStatus = sourceStatus;
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertLandListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREALandCurrentIncompleteLandDetails_Parse_ReturnsALandAvailableListing()
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeLandListing();
            expectedListing.Address.StreetNumber = "LOT 12/39";
            expectedListing.Address.DisplayAddress = expectedListing.Address.ToFormattedAddress(true, StateReplacementType.ReplaceToLongText, false, true);
            expectedListing.LandDetails.CrossOver = null;
            expectedListing.LandDetails.Depths = new List<Depth>();
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Land-Current-IncompleteLandDetails.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertLandListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREALandCurrentMissingLandCategory_Parse_ReturnsALandAvailableListing()
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeLandListing();
            expectedListing.Address.StreetNumber = "LOT 12/39";
            expectedListing.Address.DisplayAddress = expectedListing.Address.ToFormattedAddress(true, StateReplacementType.ReplaceToLongText, false, true);
            expectedListing.CategoryType = LandRuralCategoryType.Unknown;
            
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Land-Current-MissingLandCategory.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertLandListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREALandCurrentWithASubNumberButNoStreetNumber_Parse_ReturnsALandAvailableListing()
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeLandListing();
            expectedListing.Address.StreetNumber = "12";
            expectedListing.Address.DisplayAddress = expectedListing.Address.ToFormattedAddress(true, StateReplacementType.ReplaceToLongText, false, true);

            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Land-Current-WithASubNumberButNoStreetNumber.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertLandListing(result, expectedListing);
        }
    }
}