using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinances.RestAPI.Controllers;
using MyFinances.RestAPI.Data;
using MyFinances.RestAPI.Models;

namespace MyFinances.RestApi.Test;

public class WalletsControllerTests
{
    private readonly DbContextOptions<FinanceContext> _options;

    public WalletsControllerTests()
    {
        _options = new DbContextOptionsBuilder<FinanceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private async Task SeedContext(FinanceContext context)
    {
        context.Wallets.AddRange(
            new Wallet { Id = 1, Name = "Main" },
            new Wallet { Id = 2, Name = "Credit Card" }
        );
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetWallets_ReturnsAllWallets()
    {
        await using var context = new FinanceContext(_options);
        await SeedContext(context);
        var controller = new WalletsController(context);

        var result = await controller.GetWallets();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Wallet>>>(result);
        var wallets = Assert.IsAssignableFrom<IEnumerable<Wallet>>(actionResult.Value);
        Assert.Equal(2, wallets.Count());
    }

    [Fact]
    public async Task PostWallet_CreatesWallet()
    {
        await using var context = new FinanceContext(_options);
        var controller = new WalletsController(context);
        var newWallet = new Wallet { Name = "Savings" };
        await controller.PostWallet(newWallet);
        Assert.Equal(1, await context.Wallets.CountAsync());
        Assert.Equal("Savings", (await context.Wallets.FirstAsync()).Name);
    }
}