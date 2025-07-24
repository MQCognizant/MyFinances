using Microsoft.EntityFrameworkCore;
using MyFinances.RestAPI.Models;

namespace MyFinances.RestAPI.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new FinanceContext(
            serviceProvider.GetRequiredService<DbContextOptions<FinanceContext>>());

        // Look for any data.
        if (context.Wallets.Any() || context.Categories.Any() || context.Records.Any())
        {
            return; // DB has been seeded
        }

        var wallets = new[]
        {
            new Wallet { Name = "Main Bank Account" },
            new Wallet { Name = "Credit Card" },
            new Wallet { Name = "Cash" }
        };
        context.Wallets.AddRange(wallets);

        var categories = new[]
        {
            new Category { Name = "Salary" },
            new Category { Name = "Groceries" },
            new Category { Name = "Rent" },
            new Category { Name = "Utilities" },
            new Category { Name = "Transport" },
            new Category { Name = "Entertainment" }
        };
        context.Categories.AddRange(categories);

        context.SaveChanges();

        var now = DateTime.UtcNow;
        var records = new[]
        {
            new Record { Amount = 2500, RecordType = RecordType.Income, Date = new DateTime(now.Year, now.Month, 1), CategoryId = categories[0].Id, WalletId = wallets[0].Id },
            new Record { Amount = 150, RecordType = RecordType.Expense, Date = new DateTime(now.Year, now.Month, 2), CategoryId = categories[1].Id, WalletId = wallets[0].Id },
            new Record { Amount = 800, RecordType = RecordType.Expense, Date = new DateTime(now.Year, now.Month, 5), CategoryId = categories[2].Id, WalletId = wallets[0].Id },
            new Record { Amount = 75.50m, RecordType = RecordType.Expense, Date = new DateTime(now.Year, now.Month, 10), CategoryId = categories[4].Id, WalletId = wallets[1].Id },
            new Record { Amount = 45, RecordType = RecordType.Expense, Date = new DateTime(now.Year, now.Month, 12), CategoryId = categories[5].Id, WalletId = wallets[2].Id },
        };

        // Add a record for last month for testing the filter
        var lastMonth = now.AddMonths(-1);
        records = records.Append(
            new Record { Amount = 50, RecordType = RecordType.Expense, Date = new DateTime(lastMonth.Year, lastMonth.Month, 20), CategoryId = categories[1].Id, WalletId = wallets[0].Id }
        ).ToArray();

        context.Records.AddRange(records);

        context.SaveChanges();
    }
}