using Xunit;
using FoodSafety.mvc.Data;
using FoodSafety.Domain.Models;

public class InspectionTests
{
    [Fact]
    public void Score_Below_50_Is_Fail()
    {
        var inspection = new Inspection
        {
            Score = 40,
            Outcome = "Fail"
        };

        Assert.Equal("Fail", inspection.Outcome);
    }

    [Fact]
    public void Score_Above_50_Is_Pass()
    {
        var inspection = new Inspection
        {
            Score = 80,
            Outcome = "Pass"
        };

        Assert.Equal("Pass", inspection.Outcome);
    }
}