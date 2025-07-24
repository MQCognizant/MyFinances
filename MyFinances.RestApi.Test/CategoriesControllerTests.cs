using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinances.RestAPI.Controllers;
using MyFinances.RestAPI.Data;
using MyFinances.RestAPI.Models;

namespace MyFinances.RestApi.Test;

public class CategoriesControllerTests
{
    private DbContextOptions<FinanceContext> _options;

    public CategoriesControllerTests()
    {
        _options = new DbContextOptionsBuilder<FinanceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test run
            .Options;
    }

    private async Task SeedContext(FinanceContext context)
    {
        context.Categories.AddRange(
            new Category { Id = 1, Name = "Groceries" },
            new Category { Id = 2, Name = "Salary" }
        );
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetCategories_ReturnsAllCategories()
    {
        await using var context = new FinanceContext(_options);
        await SeedContext(context);
        var controller = new CategoriesController(context);

        var result = await controller.GetCategories();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Category>>>(result);
        var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(actionResult.Value);
        Assert.Equal(2, categories.Count());
    }

    [Fact]
    public async Task PostCategory_CreatesCategory()
    {
        await using var context = new FinanceContext(_options);
        var controller = new CategoriesController(context);
        var newCategory = new Category { Name = "Rent" };
        await controller.PostCategory(newCategory);
        Assert.Equal(1, await context.Categories.CountAsync());
        Assert.Equal("Rent", (await context.Categories.FirstAsync()).Name);
    }
}