using Microsoft.EntityFrameworkCore;
using MyFinances.RestAPI.Models;

namespace MyFinances.RestAPI.Data;

public class FinanceContext(DbContextOptions<FinanceContext> options) : DbContext(options)
{
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Record> Records { get; set; }
}