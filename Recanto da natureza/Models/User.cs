using System;
using System.ComponentModel.DataAnnotations;

namespace Recanto_da_natureza.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public string Phone { get; set; }

    // Armazenar hash da senha, nunca a senha em texto simples
    [Required]
    public string PasswordHash { get; set; }
}
