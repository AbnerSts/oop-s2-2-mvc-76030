using Xunit;
using FoodSafety.mvc.Data;
using FoodSafety.Domain.Models;

public class PremisesTests
{
    [Fact]
    public void Premises_Has_Name()
    {
        var p = new Premises
        {
            Name = "Cafe One"
        };

        Assert.NotNull(p.Name);
    }
}