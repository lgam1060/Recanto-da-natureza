using System;
using System.ComponentModel.DataAnnotations;

namespace Recanto_da_natureza.Models;

public class Reservation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ChaleName { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public int Nights { get; set; }

    public decimal Total { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
