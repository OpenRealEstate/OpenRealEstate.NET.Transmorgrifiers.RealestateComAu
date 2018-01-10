using OpenRealEstate.NET.Core;
using Shouldly;

namespace OpenRealEstate.NET.Transmorgrifiers.RealestateComAu.Tests
{
    public static class BuildingDetailsAssertHelpers
    {
        public static void AssertBuildingDetails(BuildingDetails source,
                                                 BuildingDetails destination)
        {
            if (source == null &&
                destination == null)
            {
                return;
            }

            UnitOfMeasureAssertHelpers.AssertUnitOfMeasure(source.Area, destination.Area);
            source.EnergyRating.ShouldBe(destination.EnergyRating);
        }
    }
}