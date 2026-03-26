using Xunit;
using Microsoft.EntityFrameworkCore;
using FoodSafety.mvc.Data;
using FoodSafety.mvc.Models;
using FoodSafety.Domain.Models;
using FoodSafety.mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

public class DashboardTests
{
    private ApplicationDbContext DbContext
    {
        get
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }

    [Fact]
    public async Task Dashboard_Counts_Work_Correctly()
    {
        var context = DbContext;

        var testDate = new DateTime(2026, 3, 1); 

        
        var premises = new Premises
        {
            Id = 1,
            Name = "Test",
            Town = "Dublin",
            RiskRating = "High"
        };
        context.Premises.Add(premises);

        
        context.Inspections.Add(new Inspection
        {
            PremisesId = 1,
            InspectionDate = testDate,
            Outcome = "Fail",
            Score = 40
        });

        context.Inspections.Add(new Inspection
        {
            PremisesId = 1,
            InspectionDate = testDate,
            Outcome = "Pass",
            Score = 90
        });

        
        context.FollowUps.Add(new FollowUp
        {
            InspectionId = 1,
            DueDate = DateTime.Now.AddDays(-2),
            Status = "Open"
        });

        await context.SaveChangesAsync();

        var controller = new DashboardController(context);

        var result = await controller.Index(default!, default!) as ViewResult;

        Assert.NotNull(result);

        Assert.Equal(2, controller.ViewBag.InspectionsThisMonth);
        Assert.Equal(1, controller.ViewBag.FailedThisMonth);
        Assert.Equal(1, controller.ViewBag.OverdueFollowUps);
    }
    }