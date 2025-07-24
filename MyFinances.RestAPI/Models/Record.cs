using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFinances.RestAPI.Models;

public class Record
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public RecordType RecordType { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int WalletId { get; set; }
    public Wallet? Wallet { get; set; }
}