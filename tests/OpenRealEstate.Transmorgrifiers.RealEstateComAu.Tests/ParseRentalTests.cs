using FluentValidation;
using OpenRealEstate.Core;
using OpenRealEstate.Core.Rental;
using OpenRealEstate.FakeData;
using OpenRealEstate.Testing;
using OpenRealEstate.Transmorgrifiers.Core;
using OpenRealEstate.Validation.Rental;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.RealEstateComAu.Tests
{
    public class ParseRentalTests : SetupTests
    {
        private const string FakeDataFolder = "Sample Data/Rental/";

        private static RentalListing CreateAFakeEmptyRentalListing(string id)
        {
            id.ShouldNotBeNullOrWhiteSpace();

            return new RentalListing
            {
                AgencyId = "XNWXNW",
                Id = id,
                CreatedOn = new DateTime(2009, 1, 1, 12, 30, 0),
                UpdatedOn = new DateTime(2009, 1, 1, 12, 30, 0)
            };
        }

        private static void AssertRentalListing(ParsedResult result,
                                                RentalListing expectedListing)
        {
            result.ShouldNotBeNull();
            result.Listings.Count.ShouldBe(1);
            result.Errors.Count.ShouldBe(0);
            result.UnhandledData.Count.ShouldBe(0);
            RentalListingAssertHelpers.AssertRuralListing(result.Listings.First().Listing as RentalListing,
                                                          expectedListing);
        }

        [Theory]
        [InlineData("REA-Rental-Current.xml")]
        [InlineData("REA-Segment-Rental-Current.xml")]
        public void GivenTheFileREARentalCurrent_Parse_ReturnsARentalAvailableListing(string fileName)
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeRentalListing();
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRentalListing(result, expectedListing);
        }

        [Theory]
        [InlineData("REA-Rental-Withdrawn.xml", "withdrawn", "Rental-Withdrawn-ABCD1234")]
        [InlineData("REA-Rental-OffMarket.xml", "offmarket", "Rental-OffMarket-ABCD1234")]
        [InlineData("REA-Rental-Deleted.xml", "deleted", "Rental-Deleted-ABCD1234")]
        public void GivenAnReaRentalFileThatRepresentsARemovedListing_Parse_ReturnsARemovedListing(string fileName,
                                                                                                   string sourceStatus,
                                                                                                   string id)
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyRentalListing(id);
            expectedListing.StatusType = StatusType.Removed;
            expectedListing.SourceStatus = sourceStatus;
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRentalListing(result, expectedListing);
        }

        [Theory]
        [InlineData("REA-Rental-Leased.xml", StatusType.Leased)]
        [InlineData("REA-Rental-Withdrawn.xml", StatusType.Removed)]
        [InlineData("REA-Rental-OffMarket.xml", StatusType.Removed)]
        [InlineData("REA-Rental-Deleted.xml", StatusType.Removed)]
        public void GivenANonCurrentReaRentalFileAndAnExistingListing_Parse_ReturnsTheUpdatedListing(string fileName,
                                                                                                     StatusType expectedStatusType)
        {
            // Arrange.
            var existingListing = FakeListings.CreateAFakeRentalListing();
            var reaXml = File.ReadAllText(FakeDataFolder + fileName);
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Arrange.
            var result = reaXmlTransmorgrifier.Parse(reaXml, existingListing);

            // Assert.
            var listing = result.Listings.First().Listing as RentalListing;
            listing.StatusType.ShouldBe(expectedStatusType);

            var validator = new RentalListingValidator();
            var validationResult = validator.Validate(listing, ruleSet: RentalListingValidator.StrictRuleSet);
            validationResult.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void GivenACurrentReaRentalFileAndAnExistingListing_Parse_ReturnsTheUpdatedListing()
        {
            // Arrange.
            var existingListing = FakeListings.CreateAFakeRentalListing();
            existingListing.Pricing.RentalPrice = 444;
            existingListing.Pricing.RentalPriceText = Guid.NewGuid().ToString();
            existingListing.Title = Guid.NewGuid().ToString();
            existingListing.Description = Guid.NewGuid().ToString();
            
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rental-Current.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();
            var newListing = reaXmlTransmorgrifier.Parse(reaXml)
                                                  .Listings
                                                  .First()
                                                  .Listing as RentalListing;

            // Arrange.
            var result = reaXmlTransmorgrifier.Parse(reaXml, existingListing);

            // Assert.
            var listing = result.Listings.First().Listing as RentalListing;
            listing.StatusType.ShouldBe(existingListing.StatusType);
            listing.Pricing.RentalPrice.ShouldBe(newListing.Pricing.RentalPrice);
            listing.Pricing.RentalPriceText.ShouldBe(newListing.Pricing.RentalPriceText);
            listing.Title.ShouldBe(newListing.Title);
            listing.Description.ShouldBe(newListing.Description);
        }

        [Fact]
        public void GivenTheFileREARentalCurrentWithNoBond_Parse_ReturnsARentalAvailableListing()
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeRentalListing();
            expectedListing.Pricing.Bond = null;
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rental-Current-WithNoBond.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRentalListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREARentalLeased_Parse_ReturnsALeasedListing()
        {
            // Arrange.
            var expectedListing = CreateAFakeEmptyRentalListing("Rental-Leased-ABCD1234");
            expectedListing.StatusType = StatusType.Leased;
            expectedListing.SourceStatus = "Leased";
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rental-Leased.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRentalListing(result, expectedListing);
        }

        [Fact]
        public void GivenTheFileREARentalWithBadStatus_Parse_ReturnsAParsedResultWithAnError()
        {
            // Arrange.
            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rental-WithBadStatus.xml");
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            result.Listings.Count.ShouldBe(0);
            result.Errors.Count.ShouldBe(1);
        }

        [Theory]
        [InlineData("week", PaymentFrequencyType.Weekly)]
        [InlineData("WeEk", PaymentFrequencyType.Weekly)] // We should be doing a case insensitive lookup.
        [InlineData("weekly", PaymentFrequencyType.Weekly)]
        [InlineData("WeEkLy", PaymentFrequencyType.Weekly)] // We should be doing a case insensitive lookup.
        [InlineData("month", PaymentFrequencyType.Monthly)]
        [InlineData("MoNtH", PaymentFrequencyType.Monthly)] // We should be doing a case insensitive lookup.
        [InlineData("monthly", PaymentFrequencyType.Monthly)]
        [InlineData("MoNtHlY", PaymentFrequencyType.Monthly)] // We should be doing a case insensitive lookup.
        [InlineData("blahblah", PaymentFrequencyType.Unknown)] // Fails to match.
        public void GivenTheFileREARentalCurrentWithAMonthlyPaymentFrequency_Parse_ReturnsARentalAvailableListing(string frequency,
                                                                                                                  PaymentFrequencyType expectedFrequency)
        {
            // Arrange.
            var expectedListing = FakeListings.CreateAFakeRentalListing();
            expectedListing.Pricing.PaymentFrequencyType = expectedFrequency;

            var reaXml = File.ReadAllText(FakeDataFolder + "REA-Rental-Current-WithFrequency.xml");
            reaXml = reaXml.Replace("XXXX-FREQUENCY", frequency); // Replace the 'template' variable with the frequency to test.
            var reaXmlTransmorgrifier = new ReaXmlTransmorgrifier();

            // Act.
            var result = reaXmlTransmorgrifier.Parse(reaXml);

            // Assert.
            AssertRentalListing(result, expectedListing);
        }
    }
}
