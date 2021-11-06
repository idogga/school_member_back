using System;
using System.ComponentModel.DataAnnotations;

public record CreatePupilDto : IUserDto
{
    [Required]
    public string Email { get; init; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public DateTime DateOfBorn { get; set; }
}