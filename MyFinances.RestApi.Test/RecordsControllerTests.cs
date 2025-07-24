using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinances.RestAPI.Controllers;
using MyFinances.RestAPI.Data;
using MyFinances.RestAPI.Models;

namespace MyFinances.RestApi.Test;

public class RecordsControllerTests
{
    private readonly DbContextOptions<FinanceContext> _options;

    public RecordsControllerTests()
    {
        _options = new DbContextOptionsBuilder<FinanceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private async Task SeedContext(FinanceContext context)
    {
        var wallet = new Wallet { Id = 1, Name = "Main" };
        var category = new Category { Id = 1, Name = "Food" };
        context.Wallets.Add(wallet);
        context.Categories.Add(category);

        var now = DateTime.UtcNow;
        context.Records.AddRange(
            new Record { Id = 1, Amount = 100, Date = new DateTime(now.Year, now.Month, 5), RecordType = RecordType.Expense, WalletId = 1, CategoryId = 1 },
            new Record { Id = 2, Amount = 200, Date = new DateTime(now.Year, now.Month, 10), RecordType = RecordType.Expense, WalletId = 1, CategoryId = 1 },
            new Record { Id = 3, Amount = 50, Date = now.AddMonths(-1), RecordType = RecordType.Expense, WalletId = 1, CategoryId = 1 } // Last month
        );
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetRecords_ReturnsRecordsForCurrentMonth()
    {
        await using var context = new FinanceContext(_options);
        await SeedContext(context);
        var controller = new RecordsController(context);
        var now = DateTime.UtcNow;

        var result = await controller.GetRecords(now.Year, now.Month, null);

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Record>>>(result);
        var records = Assert.IsAssignableFrom<IEnumerable<Record>>(actionResult.Value);
        Assert.Equal(2, records.Count());
    }

    [Fact]
    public async Task GetRecords_WithWalletFilter_ReturnsFilteredRecords()
    {
        await using var context = new FinanceContext(_options);
        var wallet1 = new Wallet { Id = 1, Name = "Main" };
        var wallet2 = new Wallet { Id = 2, Name = "Credit" };
        var category1 = new Category { Id = 1, Name = "Food" };
        context.Wallets.AddRange(wallet1, wallet2);
        context.Categories.Add(category1);
        var now = DateTime.UtcNow;
        context.Records.AddRange(
            new Record { Id = 1, Amount = 100, Date = new DateTime(now.Year, now.Month, 5), RecordType = RecordType.Expense, WalletId = 1, CategoryId = 1 },
            new Record { Id = 2, Amount = 200, Date = new DateTime(now.Year, now.Month, 10), RecordType = RecordType.Expense, WalletId = 2, CategoryId = 1 }
        );
        await context.SaveChangesAsync();
        var controller = new RecordsController(context);

        var result = await controller.GetRecords(now.Year, now.Month, 2);

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Record>>>(result);
        var records = Assert.IsAssignableFrom<IEnumerable<Record>>(actionResult.Value);
        Assert.Single(records);
        Assert.Equal(2, records.First().WalletId);
    }

    [Fact]
    public async Task PostRecord_CreatesRecord()
    {
        await using var context = new FinanceContext(_options);
        await SeedContext(context);
        var controller = new RecordsController(context);
        var newRecord = new Record { Amount = 500, Date = DateTime.UtcNow, RecordType = RecordType.Income, WalletId = 1, CategoryId = 1 };
        await controller.PostRecord(newRecord);
        Assert.Equal(4, await context.Records.CountAsync());
    }
}