using System.ComponentModel.DataAnnotations;

namespace MyFinances.RestAPI.Models;

public class Wallet
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}