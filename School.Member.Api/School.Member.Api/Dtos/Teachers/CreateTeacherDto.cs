using System;
using System.ComponentModel.DataAnnotations;

public record CreateTeacherDto : IUserDto
{
    [Required]
    public string Email { get; init; } = null!;
    
    [Required]
    public string FirstName { get; init; } = null!;
    
    [Required]
    public string LastName { get; init; } = null!;
    public string? Synonim { get; init; }
    
    [Required]
    public DateTime DateOfBorn { get; init; }
}