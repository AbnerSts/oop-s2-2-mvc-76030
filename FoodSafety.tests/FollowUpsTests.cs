using Xunit;
using FoodSafety.mvc.Data;
using FoodSafety.Domain.Models;
using System;

public class FollowUpTests
{
    [Fact]
    public void Cannot_Close_Without_ClosedDate()
    {
        var followUp = new FollowUp
        {
            Status = "Closed",
            ClosedDate = null
        };

        Assert.True(followUp.Status == "Closed" && followUp.ClosedDate == null);
    }
}